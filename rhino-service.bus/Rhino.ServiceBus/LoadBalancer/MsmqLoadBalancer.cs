using System;
using System.Messaging;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using MessageType = Rhino.ServiceBus.Msmq.MessageType;

namespace Rhino.ServiceBus.LoadBalancer
{
    public class MsmqLoadBalancer : AbstractMsmqListener
    {
        private readonly IQueueStrategy queueStrategy;
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqLoadBalancer));

        private readonly Queue<Uri> readyForWork = new Queue<Uri>();
        private readonly Set<Uri> knownWorkers = new Set<Uri>();

        public event Action<Message> MessageBatchSentToAllWorkers;

        public MsmqLoadBalancer(
            IMessageSerializer serializer,
            IQueueStrategy queueStrategy,
            IEndpointRouter endpointRouter,
            Uri endpoint,
            int threadCount)
            : base(queueStrategy, endpoint, threadCount, serializer,endpointRouter)
        {
            this.queueStrategy = queueStrategy;
        }

        protected override void BeforeStart()
        {
            try
            {
                queueStrategy.InitializeQueue(Endpoint);
            }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not open queue for load balancer: " + Endpoint + Environment.NewLine +
                    "Queue path: " + MsmqUtil.GetQueuePath(Endpoint), e);
            }

            using (var workersQueue = new MessageQueue(MsmqUtil.GetQueuePath(Endpoint.ForSubQueue(SubQueue.Workers)), QueueAccessMode.Receive))
            {
                var messages = workersQueue.GetAllMessages();
                foreach (var message in messages)
                {
                    HandleLoadBalancerMessage(message, LoadBalancerOptions.None);
                }
            }
        }

        protected override void AfterStart()
        {
            var acceptingWork = new AcceptingWork {Endpoint = Endpoint.Uri};
            SendToAllWorkers(
                GenerateMsmqMessageFromMessageBatch(acceptingWork)
                );
        }

        protected override void HandlePeekedMessage(Message message)
        {
            try
            {
                using (var tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionTimeout()))
                {
                    message = queue.TryGetMessageFromQueue(message.Id);
                    if (message == null)
                        return;

                    switch ((MessageType)message.AppSpecific)
                    {
                        case MessageType.LoadBalancerMessageMarker:
                            HandleLoadBalancerMessage(message, LoadBalancerOptions.RegisterWorkersAsReadyToWork | LoadBalancerOptions.RegisterWorkersInQueue);
                            break;
                        case MessageType.AdministrativeMessageMarker:
                            SendToAllWorkers(message);
                            break;
                        default:
                            HandleStandardMessage(message);
                            break;
                    }
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                logger.Error("Fail to process load balanced message properly", e);
            }
        }

        private void HandleStandardMessage(Message message)
        {
            var worker = readyForWork.Dequeue();

            if (worker == null) // handle message later
            {
                queue.Send(message, queue.GetTransactionType());
            }
            else
            {
                var workerEndpoint = endpointRouter.GetRoutedEndpoint(worker);
                using (var workerQueue = new MessageQueue(MsmqUtil.GetQueuePath(workerEndpoint), QueueAccessMode.Send))
                {
                    workerQueue.Send(message, workerQueue.GetTransactionType());
                }
            }
        }

        private void SendToAllWorkers(Message message)
        {
            var values = knownWorkers.GetValues();
            foreach (var worker in values)
            {
                var workerEndpoint = endpointRouter.GetRoutedEndpoint(worker);
                using (var workerQueue = new MessageQueue(MsmqUtil.GetQueuePath(workerEndpoint), QueueAccessMode.Send))
                {
                    workerQueue.Send(message, workerQueue.GetTransactionType());
                }
            }
            if (values.Length == 0) 
                return;

            var copy = MessageBatchSentToAllWorkers;
            if (copy != null)
                copy(message);
        }

        private void HandleLoadBalancerMessage(Message message, LoadBalancerOptions options)
        {
            foreach (var msg in DeserializeMessages(queue, message, null))
            {
                var work = msg as ReadyToWork;

                if (work == null)
                    continue;

                var needToAddToQueue = knownWorkers.Add(work.Endpoint);

                var shouldRegisterInQueue = (options & LoadBalancerOptions.RegisterWorkersInQueue) == LoadBalancerOptions.RegisterWorkersInQueue;
                if (needToAddToQueue && shouldRegisterInQueue)
                {
                    queue.Send(message.SetSubQueueToSendTo(SubQueue.Workers), queue.GetTransactionType());
                }

                if ((options & LoadBalancerOptions.RegisterWorkersAsReadyToWork) == LoadBalancerOptions.RegisterWorkersAsReadyToWork)
                    readyForWork.Enqueue(work.Endpoint);
            }
        }

        [Flags]
        private enum LoadBalancerOptions
        {
            None = 0,
            RegisterWorkersInQueue = 1,
            RegisterWorkersAsReadyToWork = 2
        }
    }
}