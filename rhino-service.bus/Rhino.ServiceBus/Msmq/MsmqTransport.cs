using System;
using System.Collections.Generic;
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
    public class MsmqTransport : ITransport
    {
        [ThreadStatic]
        private static MsmqCurrentMessageInformation currentMessageInformation;

        private readonly Uri endpoint;
        private readonly Dictionary<string, ErrorCounter> failureCounts = new Dictionary<string, ErrorCounter>();
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqTransport));
        private readonly int numberOfRetries;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IMessageSerializer serializer;
        private readonly int threadCount;
        private readonly WaitHandle[] waitHandles;
        private MessageQueue queue;
        private bool haveStarted;
        private const int AdministrativeMessageMarker = 42;

        public MsmqTransport(
            IMessageSerializer serializer,
            Uri endpoint,
            int threadCount,
            int numberOfRetries)
        {
            this.serializer = serializer;
            this.endpoint = endpoint;
            this.threadCount = threadCount;
            this.numberOfRetries = numberOfRetries;
            waitHandles = new WaitHandle[threadCount + 1];
        }

        public bool ShouldStop { get; set; }

        #region ITransport Members

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        public void Start()
        {
            queue = InitalizeQueue(endpoint);

            for (int t = 0; t < threadCount; t++)
            {
                var waitHandle = new ManualResetEvent(false);
                waitHandles[t] = waitHandle;
                try
                {
                    queue.BeginPeek(TimeSpan.FromSeconds(1), new QueueState
                    {
                        Queue = queue,
                        WaitHandle = waitHandle
                    }, OnPeekMessage);
                }
                catch (Exception e)
                {
                    throw new TransportException("Unable to start reading from queue: " + endpoint, e);
                }
            }
            haveStarted = true;
        }

        public void Stop()
        {
            ShouldStop = true;

            WaitForProcessingToEnd();

            queue.Close();
        }

        private void WaitForProcessingToEnd()
        {
            if (haveStarted == false)
                return;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
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

        public void Reply(params object[] messages)
        {
            if (currentMessageInformation == null)
                throw new TransactionException("There is no message to reply to, sorry.");

            Send(currentMessageInformation.Source, messages);
        }

        public event Action<CurrentMessageInformation> AdministrativeMessageArrived;
        public event Action<CurrentMessageInformation> MessageArrived;
        public event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;
        public event Action<CurrentMessageInformation> MessageProcessingCompleted;

        public void Send(Uri uri, params object[] msgs)
        {
            var message = new Message();

            serializer.Serialize(msgs, new MsmqTransportMessage(message));

            message.ResponseQueue = queue;

            SetCorrelationIdOnMessage(message);

            message.AppSpecific =
                msgs[0] is AdministrativeMessage ? AdministrativeMessageMarker : 0;

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

            SendMessageToQueue(message, uri);
        }

        #endregion

        private static MessageQueue InitalizeQueue(Uri endpoint)
        {
            QueueDescriptor queueDescriptor = MsmqUtil.GetQueueDescription(endpoint);
            try
            {
                var messageQueue = new MessageQueue(queueDescriptor.QueuePath, QueueAccessMode.SendAndReceive);
                var filter = new MessagePropertyFilter();
                filter.SetAll();
                messageQueue.MessageReadPropertyFilter = filter;
                return messageQueue;
            }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not receive from queue: " + endpoint + Environment.NewLine +
                    "Queue path: " + queueDescriptor.QueuePath, e);
            }
        }

        private void OnPeekMessage(IAsyncResult ar)
        {
            Message message;
            bool? peek = TryEndingPeek(ar, out message);
            if (peek == false) // error 
                return;

            var state = (QueueState)ar.AsyncState;
            if (ShouldStop)
            {
                state.WaitHandle.Set();
                return;
            }

            if (peek == null) //nothing was found
            {
                state.Queue.BeginPeek(TimeSpan.FromSeconds(1), state, OnPeekMessage);
                return;
            }

            if (DispatchToErrorQueueIfNeeded(state.Queue, message))
            {
                state.Queue.BeginPeek(TimeSpan.FromSeconds(1), state, OnPeekMessage);
                return;
            }

            if (HandleAdministrationMessage(state, message))
            {
                state.Queue.BeginPeek(TimeSpan.FromSeconds(1), state, OnPeekMessage);
                return;
            }

            ReceiveMessageInTransaction(state);

            state.Queue.BeginPeek(TimeSpan.FromSeconds(1), state, OnPeekMessage);
        }

        private bool HandleAdministrationMessage(QueueState state, Message message)
        {
            if (message.AppSpecific != AdministrativeMessageMarker)
                return false;
            Exception ex = null;
            try
            {
                ProcessMessage(message, state.Queue, AdministrativeMessageArrived);
            }
            catch (Exception e)
            {
                ex = e;
                logger.Error("Failed to process administrative message", e);
            }
            finally
            {
                HandleMessageCompletion(message, null, state, ex);
            }
            return true;
        }

        private void ReceiveMessageInTransaction(QueueState state)
        {
            using (var tx = new TransactionScope())
            {
                Message message = null;
                Exception ex = null;
                try
                {
                    state.Queue.MessageReadPropertyFilter.SetAll();
                    message = state.Queue.Receive(
                        TimeSpan.FromSeconds(5),
                        GetTransactionTypeBasedOnQueue(state.Queue));

                    ProcessMessage(message, state.Queue, MessageArrived);
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    {
                        return; // someone got it before we did, better luck next time
                    }
                    else
                    {
                        ex = e;
                        logger.Error("Failed to receive message", e);
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                    logger.Error("Failed to receive message", e);
                }
                finally
                {
                    HandleMessageCompletion(message, tx, state, ex);
                }
            }
        }

        private bool DispatchToErrorQueueIfNeeded(MessageQueue messageQueue, Message message)
        {
            string id = GetMessageId(message);

            readerWriterLock.EnterReadLock();
            ErrorCounter errorCounter;
            try
            {
                if (failureCounts.TryGetValue(id, out errorCounter) == false)
                    return false;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            if (errorCounter.FailureCount < numberOfRetries)
                return false;

            readerWriterLock.EnterWriteLock();
            try
            {
                failureCounts.Remove(id);
                //not sure how to do this when moving queue
                //message.Extension = Encoding.Unicode.GetBytes(errorCounter.ExceptionText);
                messageQueue.MoveToSubQueue("errors", message);
                logger.WarnFormat("Moving message {0} to errors subqueue because: {1}", message.Id, errorCounter.ExceptionText);
                return true;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        private static MessageQueueTransactionType GetTransactionTypeBasedOnQueue(MessageQueue theQueueToCheck)
        {
            if (theQueueToCheck.Transactional == false)
                return MessageQueueTransactionType.None;

            return Transaction.Current == null
                       ? MessageQueueTransactionType.Single
                        : MessageQueueTransactionType.Automatic;
        }

        private void HandleMessageCompletion(
            Message message,
            TransactionScope tx,
            QueueState state,
            Exception exception)
        {
            if (exception == null)
            {
                try
                {
                    if (tx != null)
                        tx.Complete();
                    if (message != null)
                        RemoveMessageFromFailureTracking(message);
                    return;
                }
                catch (Exception e)
                {
                    logger.Warn("Failed to complete transaction, moving to error mode", e);
                }
            }
            if (message == null)
                return;
            IncrementFailureCount(GetMessageId(message), exception);
            if (state.Queue.Transactional == false)
            {
                state.Queue.Send(message, MessageQueueTransactionType.None);
            }
        }

        private static string GetMessageId(Message message)
        {
            string id = message.Id;
            return id.Split('\\')[0];
        }

        private void IncrementFailureCount(string id, Exception e)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                ErrorCounter errorCounter;
                if (failureCounts.TryGetValue(id, out errorCounter) == false)
                {
                    errorCounter = new ErrorCounter
                    {
                        ExceptionText = e.ToString(),
                        FailureCount = 0
                    };
                    failureCounts[id] = errorCounter;
                }
                errorCounter.FailureCount += 1;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        private void RemoveMessageFromFailureTracking(Message message)
        {
            string id = GetMessageId(message);
            readerWriterLock.EnterReadLock();
            try
            {
                if (failureCounts.ContainsKey(id) == false)
                    return;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                failureCounts.Remove(id);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

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
            return true;
        }

        private void ProcessMessage(
            Message message,
            MessageQueue theQueue,
            Action<CurrentMessageInformation> messageRecieved)
        {
            var transportMessage = new MsmqTransportMessage(message);
            object[] messages = DeserializeMessages(transportMessage);
            try
            {
                foreach (var msg in messages)
                {
                    currentMessageInformation = new MsmqCurrentMessageInformation
                    {
                        TransactionType = GetTransactionTypeBasedOnQueue(theQueue),
                        MsmqMessage = message,
                        Queue = theQueue,
                        TransportMessage = transportMessage,
                        MessageId = CorrelationId.Parse(message.Id),
                        AllMessages = messages,
                        CorrelationId = CorrelationId.Parse(message.CorrelationId),
                        Message = msg,
                        Source = MsmqUtil.GetQueueUri(message.ResponseQueue)
                    };

                    if (messageRecieved != null)
                        messageRecieved(currentMessageInformation);
                }
            }
            catch (Exception e)
            {
                try
                {
                    Action<CurrentMessageInformation, Exception> copy = MessageProcessingFailure;
                    if (copy != null)
                        copy(currentMessageInformation, e);
                }
                catch (Exception moduleException)
                {
                    throw new TransportException("Module failed to process message failure: " + e.Message,
                                                 moduleException);
                }
                throw;
            }
            finally
            {
                Action<CurrentMessageInformation> copy = MessageProcessingCompleted;
                if (copy != null)
                    copy(currentMessageInformation);
                currentMessageInformation = null;
            }
        }

        private object[] DeserializeMessages(ITransportMessage transportMessage)
        {
            object[] messages;
            try
            {
                messages = serializer.Deserialize(transportMessage);
            }
            catch (Exception e)
            {
                try
                {
                    logger.Error("Error when serializing message", e);
                    var copy = MessageSerializationException;
                    if (copy != null)
                        copy(transportMessage,e);
                }
                catch (Exception moduleEx)
                {
                    logger.Error("Error when notifying about serialization exception", moduleEx);
                }
                throw;
            }
            return messages;
        }

        public event Action<ITransportMessage, Exception> MessageSerializationException;

        private static void SetCorrelationIdOnMessage(Message message)
        {
            if (currentMessageInformation != null)
                message.CorrelationId = currentMessageInformation.CorrelationId
                    .Increment().ToString();
        }

        private static void SendMessageToQueue(Message message, Uri uri)
        {
            QueueDescriptor sendQueueDescription = MsmqUtil.GetQueueDescription(uri);
            try
            {
                using (var sendQueue = new MessageQueue(
                    sendQueueDescription.QueuePath,
                    QueueAccessMode.Send))
                {
                    MessageQueueTransactionType transactionType = GetTransactionTypeBasedOnQueue(sendQueue);
                    sendQueue.Send(message, transactionType);
                }
            }
            catch (Exception e)
            {
                throw new TransactionException("Failed to send message to " + uri, e);
            }
        }

        #region Nested type: ErrorCounter

        private class ErrorCounter
        {
            public string ExceptionText;
            public int FailureCount;
        }

        #endregion

        #region Nested type: QueueState

        private class QueueState
        {
            public MessageQueue Queue;
            public ManualResetEvent WaitHandle;
        }

        #endregion
    }
}