using System;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;

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

        private readonly IMessageSerializer messageSerializer;
        private readonly ILog logger = LogManager.GetLogger(typeof(AbstractMsmqListener));

        private readonly int threadCount;
        public event Action MessageMoved;

        protected AbstractMsmqListener(
            IQueueStrategy queueStrategy, 
            Uri endpoint, 
            int threadCount, 
            IMessageSerializer messageSerializer,
            IEndpointRouter endpointRouter)
        {
            this.queueStrategy = queueStrategy;
            this.messageSerializer = messageSerializer;
            this.endpointRouter = endpointRouter;
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

        public Endpoint Endpoint
        {
            get
            {
                return endpointRouter.GetRoutedEndpoint(endpoint);
            }
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
            queue = InitalizeQueue(Endpoint);

            BeforeStart();

            shouldStop = false;
            TransportState = TransportState.Started;

            for (var t = 0; t < ThreadCount; t++)
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

            AfterStart();
        }

        protected virtual void AfterStart()
        {
            
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

        protected static MessageQueue InitalizeQueue(Endpoint endpoint)
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

                    if (peek == false || shouldStop)//error reading from queue
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

        protected IEndpointRouter endpointRouter;

        public TransportState TransportState { get; set; }

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

        protected Message GenerateMsmqMessageFromMessageBatch(params object[] msgs)
        {
            var message = new Message();

            messageSerializer.Serialize(msgs, message.BodyStream);

            message.ResponseQueue = queue;

            message.Extension = Guid.NewGuid().ToByteArray();

            message.AppSpecific = GetAppSpecificMarker(msgs);

            message.Label = msgs
                .Where(msg => msg != null)
                .Select(msg =>
                {
                    string s = msg.ToString();
                    if (s.Length > 249)
                        return s.Substring(0, 246) + "...";
                    return s;
                })
                .FirstOrDefault();
            return message;
        }

        protected static int GetAppSpecificMarker(object[] msgs)
        {
            var msg = msgs[0];
            if (msg is AdministrativeMessage)
                return (int)MessageType.AdministrativeMessageMarker;
            if (msg is LoadBalancerMessage)
                return (int)MessageType.LoadBalancerMessageMarker;
            return 0;
        }

        protected object[] DeserializeMessages(MessageQueue messageQueue, Message transportMessage, Action<CurrentMessageInformation, Exception> messageSerializationException)
        {
            try
            {
                return messageSerializer.Deserialize(transportMessage.BodyStream);
            }
            catch (Exception e)
            {
                try
                {
                    logger.Error("Error when serializing message", e);
                    if (messageSerializationException != null)
                    {
                        var information = new MsmqCurrentMessageInformation
                        {
                            MsmqMessage = transportMessage,
                            Queue = messageQueue,
                            Message = transportMessage,
                            Source = MsmqUtil.GetQueueUri(messageQueue),
                            MessageId = transportMessage.GetMessageId()
                        };
                        messageSerializationException(information, e);
                    }
                }
                catch (Exception moduleEx)
                {
                    logger.Error("Error when notifying about serialization exception", moduleEx);
                }
                throw;
            }
        }
    }
}
