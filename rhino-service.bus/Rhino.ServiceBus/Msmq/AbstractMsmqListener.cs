using System;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using log4net;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
    public abstract class AbstractMsmqListener
    {
        protected Uri endpoint;
        protected WaitHandle[] waitHandles;
        protected bool haveStarted;
        protected MessageQueue queue;
        public volatile bool ShouldStop;
        private readonly ILog logger = LogManager.GetLogger(typeof (AbstractMsmqListener));

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        protected static TimeSpan TimeOutForPeek
        {
            get { return TimeSpan.FromHours(1); }
        }

        public void Stop()
        {
            ShouldStop = true;

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
            if (ShouldStop ||
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
                    if(ShouldStop)
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
    }
}