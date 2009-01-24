using System;
using System.Collections;
using System.Messaging;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer.Messages;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus.LoadBalancer.Msmq
{
    public class MsmqLoadBalancer : AbstractMsmqListener, ILoadBalancer
    {
        private readonly IMessageSerializer serializer;
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqLoadBalancer));

        private readonly Queue<Uri> readyForWork = new Queue<Uri>();

        public MsmqLoadBalancer(
            IMessageSerializer serializer,
            Uri endpoint, int threadCount)
            : base(endpoint, threadCount)
        {
            this.serializer = serializer;
        }

        protected override void BeforeStart()
        {
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