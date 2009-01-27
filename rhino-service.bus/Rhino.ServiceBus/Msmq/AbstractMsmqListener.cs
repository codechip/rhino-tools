using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
    public abstract class AbstractMsmqListener : IDisposable
    {
        private readonly IQueueStrategy queueStrategy;
        private readonly Uri endpoint;
        private readonly Thread[] threads;
        private bool haveStarted;

        protected MessageQueue queue;

        private volatile bool shouldStop;

        private readonly ILog logger = LogManager.GetLogger(typeof(AbstractMsmqListener));

        private readonly int threadCount;
        public event Action MessageMoved;

        protected AbstractMsmqListener(IQueueStrategy queueStrategy, Uri endpoint, int threadCount)
        {
            this.queueStrategy = queueStrategy;
            this.endpoint = endpoint;
            this.threadCount = threadCount;
            threads = new Thread[threadCount];
        }

        public event Action Started;

        public bool HaveStarted
        {
            get { return haveStarted; }
        }

        public int ThreadCount
        {
            get { return threadCount; }
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        protected static TimeSpan TimeOutForPeek
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        public void Start()
        {
            if (haveStarted)
                return;
            logger.DebugFormat("Starting msmq transport on: {0}", Endpoint);
            queue = InitalizeQueue(endpoint);

            BeforeStart();

            shouldStop = false;
            TransportState = TransportState.Started;

            for (var t = 0; t < threadCount; t++)
            {
                var thread = new Thread(PeekMessageOnBackgroundThread)
                {
                    Name = "Rhino Service Bus Worker Thread #" + t,
                    IsBackground = true
                };
                threads[t] = thread;
                thread.Start();
            }

            haveStarted = true;

            var copy = Started;
            if (copy != null)
                copy();
        }

        protected virtual void BeforeStart()
        {

        }

        public void Dispose()
        {
            shouldStop = true;

            queue.Send(new Message
            {
                Label = "Shutdown bus",
                AppSpecific = (int)MessageType.ShutDownMessageMarker
            }, queue.GetSingleMessageTransactionType());

            if (queue != null)
                queue.Close();

            WaitForProcessingToEnd();

            haveStarted = false;
            TransportState = TransportState.Stopped;
        }

        private void WaitForProcessingToEnd()
        {
            if (haveStarted == false)
                return;

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        protected static MessageQueue InitalizeQueue(Uri endpoint)
        {
            try
            {
                var queue = new MessageQueue(MsmqUtil.GetQueuePath(endpoint), QueueAccessMode.SendAndReceive);
                queue.MessageReadPropertyFilter.SetAll();
                return queue;
            }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not open queue: " + endpoint + Environment.NewLine +
                    "Queue path: " + MsmqUtil.GetQueuePath(endpoint) +
                    "Did you create the queue or disable the queue initialization module?", e);
            }

        }

        protected void PeekMessageOnBackgroundThread()
        {
            while (shouldStop == false)
            {
                try
                {
                    Message message;
                    bool? peek = TryPeek(out message);

                    if (peek == false)//error reading from queue
                    {
                        TransportState = TransportState.FailedToReadFromQueue;
                        return; // return from method, we have failed}
                    }
                    if (peek == null) //nothing was found 
                        continue;

                    if ((MessageType)((message.AppSpecific & 0xFFFF0000) >> 16) == MessageType.MoveMessageMarker)
                    {
                        var subQueue = (SubQueue)(0x0000FFFF & message.AppSpecific);
                        using (var tx = new TransactionScope())
                        {
                            string msgId;
                            queueStrategy.TryMoveMessage(queue, message, subQueue, out msgId);
                            tx.Complete();
                        }
                        var copy = MessageMoved;
                        if (copy != null)
                            copy();
                        continue;
                    }
                    logger.DebugFormat("Got message {0} from {1}",
                                       message.Label,
                                       MsmqUtil.GetQueueUri(queue));

                    HandlePeekedMessage(message);
                }
                catch (Exception e)
                {
                    Debugger.Break();
                    Debug.Fail("should not happen", e.ToString());
                }
            }

        }

        private TransportState transportState;

        public TransportState TransportState
        {
            get { return transportState; }
            set { transportState = value; }
        }

        protected abstract void HandlePeekedMessage(Message message);


        private bool? TryPeek(out Message message)
        {
            try
            {
                message = queue.Peek(TimeOutForPeek);
            }
            catch (MessageQueueException e)
            {
                message = null;
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    logger.Error("Could not peek message from queue", e);
                    return false;
                }
                return null; // nothing found
            }
            catch (Exception e)
            {
                message = null;
                logger.Error("Could not peek message from queue", e);
                return false;
            }
            return true;
        }

        protected static TimeSpan GetTransactionTimeout()
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromMinutes(45);
            return TimeSpan.Zero;
        }
    }
}
