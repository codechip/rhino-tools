using System;
using System.Messaging;
using System.Threading;
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
        public Uri SecondaryLoadBalancer { get; set; }
        public Uri PrimaryLoadBalancer { get; set; }
        private readonly IQueueStrategy queueStrategy;
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqLoadBalancer));

        private readonly Queue<Uri> readyForWork = new Queue<Uri>();
        private readonly Set<Uri> knownWorkers = new Set<Uri>();
        private readonly Timer heartBeatTimer;
        private readonly Set<Uri> knownEndpoints = new Set<Uri>();

        public event Action<Message> MessageBatchSentToAllWorkers;

        public MsmqLoadBalancer(
            IMessageSerializer serializer,
            IQueueStrategy queueStrategy,
            IEndpointRouter endpointRouter,
            Uri endpoint,
            int threadCount)
            : base(queueStrategy, endpoint, threadCount, serializer, endpointRouter)
        {
            heartBeatTimer = new Timer(SendHeartBeatToSecondaryServer);
            this.queueStrategy = queueStrategy;
        }

        protected void SendHeartBeatToSecondaryServer(object ignored)
        {
            try
            {
                using (var secondaryLoadBalancerQueue = new MessageQueue(MsmqUtil.GetQueuePath(new Endpoint { Uri = SecondaryLoadBalancer })))
                {
                    var message = GenerateMsmqMessageFromMessageBatch(new HeartBeat
                    {
                        From = Endpoint.Uri,
                        At = DateTime.Now,
                    });
                    secondaryLoadBalancerQueue.Send(message);
                }
            }
            catch (Exception e)
            {
                throw new LoadBalancerException("Could not send heat beat to secondary load balancer: " + SecondaryLoadBalancer, e);
            }
        }


        public Set<Uri> KnownWorkers
        {
            get { return knownWorkers; }
        }

        public Set<Uri> KnownEndpoints
        {
            get { return knownEndpoints; }
        }

        public int NumberOfWorkersReadyToHandleMessages
        {
            get { return readyForWork.TotalCount; }
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

            ReadUrisFromSubQueue(KnownWorkers, SubQueue.Workers);

            ReadUrisFromSubQueue(KnownEndpoints, SubQueue.Endpoints);

            RemoveAllReadyToWorkMessages();
        }

        private void ReadUrisFromSubQueue(Set<Uri> set, SubQueue subQueue)
        {
            using (var messageQueue = new MessageQueue(MsmqUtil.GetQueuePath(Endpoint.ForSubQueue(subQueue)), QueueAccessMode.Receive))
            {
                messageQueue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                var messages = messageQueue.GetAllMessages();
                foreach (var message in messages)
                {
                    var uriString = message.Body.ToString();
                    set.Add(new Uri(uriString));
                }
            }
        }


        private void RemoveAllReadyToWorkMessages()
        {
            using(var tx = new TransactionScope())
            using (var readyForWorkQueue = new MessageQueue(MsmqUtil.GetQueuePath(Endpoint), QueueAccessMode.Receive))
            using (var enumerator = readyForWorkQueue.GetMessageEnumerator2())
            {
                try
                {
                    while (enumerator.MoveNext())
                    {
                        while (
                            enumerator.Current != null &&
                            enumerator.Current.Label == typeof(ReadyToWork).FullName)
                        {
                            var current = enumerator.RemoveCurrent(readyForWorkQueue.GetTransactionType());
                            HandleLoadBalancerMessage(current);
                        }
                    }
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                        throw;
                }
                readyForWork.Clear();
                tx.Complete();
            }
        }

        protected override void AfterStart()
        {
            if (SecondaryLoadBalancer != null)
            {
                SendHeartBeatToSecondaryServer(null);
                heartBeatTimer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }

            var acceptingWork = new AcceptingWork { Endpoint = Endpoint.Uri };
            SendToAllWorkers(
                GenerateMsmqMessageFromMessageBatch(acceptingWork)
                );
        }

        protected override void OnStop()
        {
            heartBeatTimer.Dispose();
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

                    PersistEndPoint(message);

                    switch ((MessageType)message.AppSpecific)
                    {
                        case MessageType.LoadBalancerMessageMarker:
                            HandleLoadBalancerMessage(message);
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

        private void PersistEndPoint(Message message)
        {
            var queueUri = MsmqUtil.GetQueueUri(message.ResponseQueue);
            bool needToPersist = knownEndpoints.Add(queueUri);
            if (needToPersist)
            {
                var persistedEndPoint = new Message
                {
                    Formatter = new XmlMessageFormatter(new[] { typeof(string) }),
                    Body = queueUri.ToString(),
                    Label = ("Known end point: " + queueUri).EnsureLabelLength()
                };
                queue.Send(persistedEndPoint.SetSubQueueToSendTo(SubQueue.Endpoints), queue.GetTransactionType());
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
            var values = KnownWorkers.GetValues();
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

        private void HandleLoadBalancerMessage(Message message)
        {
            foreach (var msg in DeserializeMessages(queue, message, null))
            {
                var work = msg as ReadyToWork;

                if (work == null)
                    continue;

                var needToAddToQueue = KnownWorkers.Add(work.Endpoint);

                if (needToAddToQueue)
                {
                    var persistedWorker = new Message
                    {
                        Formatter = new XmlMessageFormatter(new[] { typeof(string) }),
                        Body = work.Endpoint.ToString(),
                        Label = ("Known worker: " + work.Endpoint).EnsureLabelLength()
                    };
                    queue.Send(persistedWorker.SetSubQueueToSendTo(SubQueue.Workers), queue.GetTransactionType());
                }

                readyForWork.Enqueue(work.Endpoint);
            }
        }
    }
}
