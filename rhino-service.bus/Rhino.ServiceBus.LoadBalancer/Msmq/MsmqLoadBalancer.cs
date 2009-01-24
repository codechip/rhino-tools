using System;
using System.Messaging;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using MessageType=Rhino.ServiceBus.Msmq.MessageType;

namespace Rhino.ServiceBus.LoadBalancer.Msmq
{
    public class MsmqLoadBalancer : AbstractMsmqListener
    {
        private readonly IMessageSerializer serializer;
        private readonly IQueueStrategy queueStrategy;
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqLoadBalancer));

        private readonly Queue<Uri> readyForWork = new Queue<Uri>();

        public MsmqLoadBalancer(
            IMessageSerializer serializer,
            IQueueStrategy queueStrategy,
            Uri endpoint, 
            int threadCount)
            : base(endpoint, threadCount)
        {
            this.serializer = serializer;
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

            using (var workersQueue = new MessageQueue(MsmqUtil.GetQueuePath(Endpoint + ";Workers"), QueueAccessMode.Receive))
            {
                var messages = workersQueue.GetAllMessages();
                foreach (var message in messages)
                {
                    HandleLoadBalancerMessage(message, false);
                }
            }
        }

        protected override void HandlePeekedMessage(QueueState state, Message message)
        {
            try
            {
                if (message.AppSpecific == (int)MessageType.LoadBalancerMessage)
                {
                    HandleLoadBalancerMessage(message, true);
                    return;
                }

                using(var tx = new  TransactionScope(TransactionScopeOption.Required, GetTransactionTimeout()))
                {
                    message = state.Queue.TryGetMessageFromQueue(message.Id);
                    var worker = readyForWork.Dequeue();

                    if(worker==null)// handle message later
                    {
                        state.Queue.Send(message);
                    }
                    else
                    {
                        using(var workerQueue = new MessageQueue(MsmqUtil.GetQueuePath(worker),QueueAccessMode.Send))
                        {
                            workerQueue.Send(message, workerQueue.GetTransactionType());
                        }
                    }
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                logger.Error("Fail to process message properly", e);
            }
        }

        private void HandleLoadBalancerMessage(Message message, bool moveToWorkersQueue)
        {
            object[] msgs;
            try
            {
                msgs = serializer.Deserialize(message.BodyStream);
            }
            catch (Exception e)
            {
                logger.Error("Failed to deserialize message", e);
                return;
            }

            try
            {
                if (moveToWorkersQueue)
                    queue.MoveToSubQueue("Workers", message);
            }
            catch (TransportException)
            {
                return;// probably picked by another thread
            }
            foreach (var msg in msgs)
            {
                var work = msg as ReadyToWork;
                if (work != null)
                {
                    readyForWork.Enqueue(work.Endpoint);
                }
            }
        }
    }
}