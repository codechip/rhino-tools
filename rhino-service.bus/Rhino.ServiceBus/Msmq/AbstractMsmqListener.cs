using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using log4net;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
    public abstract class AbstractMsmqListener : IDisposable
    {
        private readonly Uri endpoint;
        private readonly WaitHandle[] waitHandles;
        private bool haveStarted;

        protected MessageQueue queue;

        private volatile bool shouldStop;

        private readonly ILog logger = LogManager.GetLogger(typeof (AbstractMsmqListener));

        private readonly int threadCount;

        protected AbstractMsmqListener(Uri endpoint, int threadCount)
        {
            this.endpoint = endpoint;
            this.threadCount = threadCount;
            waitHandles = new WaitHandle[threadCount];
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
            get { return TimeSpan.FromHours(1); }
        }

        public void Start()
        {
            if (haveStarted)
                return;

            logger.DebugFormat("Starting msmq transport on: {0}", Endpoint);
            queue = InitalizeQueue(endpoint);

            BeforeStart();

            for (var t = 0; t < threadCount; t++)
            {
                var waitHandle = new ManualResetEvent(true);
                waitHandles[t] = waitHandle;
                try
                {
                    queue.BeginPeek(TimeOutForPeek, new QueueState
                    {
                        Queue = queue,
                        WaitHandle = waitHandle
                    }, OnPeekMessage);
                    waitHandle.Reset();
                }
                catch (Exception e)
                {
                    throw new TransportException("Unable to start reading from queue: " + endpoint, e);
                }
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

            if (queue != null)
                queue.Close();
            
            WaitForProcessingToEnd();

            haveStarted = false;
        }

        private void WaitForProcessingToEnd()
        {
            if (haveStarted == false)
                return;

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                WaitHandle.WaitAll(waitHandles);
            }
            else
            {
                foreach (WaitHandle handle in waitHandles)
                {
                    if (handle != null)
                        handle.WaitOne();
                }
            }
        }

        protected static MessageQueue InitalizeQueue(Uri endpoint)
        {
            try
            {
                var queue = new MessageQueue(MsmqUtil.GetQueuePath(endpoint), QueueAccessMode.SendAndReceive);
                var filter = new MessagePropertyFilter();
                filter.SetAll();
                queue.MessageReadPropertyFilter = filter;
                return queue;
            }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not open queue: " + endpoint + Environment.NewLine +
                    "Queue path: " + MsmqUtil.GetQueuePath(endpoint) +
                    "Did you create the queue or disable the queue initialization module?",e);
            }
			
        }

        protected void OnPeekMessage(IAsyncResult ar)
        {
            Message message;
            var state = (QueueState)ar.AsyncState;
            bool? peek = TryEndingPeek(ar, out message);
            if (shouldStop ||
                peek == false)// error peeking from queue
            {
                state.WaitHandle.Set();
                return;
            }

            try
            {
                if (peek == null)//nothing was found 
                    return;

                logger.DebugFormat("Got message {0} from {1}",
                                   message.Label,
                                   MsmqUtil.GetQueueUri(state.Queue));

                HandlePeekedMessage(state, message);
            }
            finally
            {
                try
                {
                    if(shouldStop)
                    {
                        state.WaitHandle.Set();
                    }
                    else
                    {
                        state.Queue.BeginPeek(TimeOutForPeek, state, OnPeekMessage);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Could not begin peeking from queue", e);
                    state.WaitHandle.Set();
                }
            }
        }

        protected abstract void HandlePeekedMessage(QueueState state, Message message);

       
        private bool? TryEndingPeek(IAsyncResult ar, out Message message)
        {
            var state = (QueueState)ar.AsyncState;
            try
            {
                message = state.Queue.EndPeek(ar);
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
            catch(Exception e)
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