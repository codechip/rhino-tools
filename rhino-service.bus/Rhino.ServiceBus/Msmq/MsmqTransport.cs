using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using System.Linq;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqTransport : ITransport
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqTransport));
        private readonly MessageQueue queue;
        private readonly QueueDescriptor queueDescriptor;
        private readonly IMessageSerializer serializer;
        private readonly Uri endpoint;
        private readonly Uri errorEndpoint;
        private readonly int threadCount;
        private readonly int numberOfRetries;
        private readonly WaitHandle[] waitHandles;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, ErrorCounter> failureCounts = new Dictionary<string, ErrorCounter>();

        [ThreadStatic]
        private static CurrentMessageInformation currentMessageInformation;

        private class ErrorCounter
        {
            public string ExceptionText;
            public int FailureCount;
        }

        public MsmqTransport(
            IMessageSerializer serializer,
            Uri endpoint,
            Uri errorEndpoint,
            int threadCount,
            int numberOfRetries)
        {
            this.serializer = serializer;
            this.endpoint = endpoint;
            this.errorEndpoint = errorEndpoint;
            this.threadCount = threadCount;
            this.numberOfRetries = numberOfRetries;
            waitHandles = new WaitHandle[threadCount];
            queueDescriptor = MsmqUtil.GetQueueDescription(endpoint);
            try
            {
                queue = new MessageQueue(queueDescriptor.QueuePath, QueueAccessMode.SendAndReceive);
                var filter = new MessagePropertyFilter();
                filter.SetAll();
                queue.MessageReadPropertyFilter = filter;

            }
            catch (Exception e)
            {
                throw new TransportException(
                    "Could not receive from queue: " + endpoint + Environment.NewLine +
                    "Queue path: " + queueDescriptor.QueuePath, e);
            }
        }

        public Uri Endpoint
        {
            get { return endpoint; }
        }

        public bool ShouldStop { get; set; }

        #region ITransport Members

        public void Start()
        {
            for (int t = 0; t < threadCount; t++)
            {
                var waitHandle = new ManualResetEvent(false);
                waitHandles[t] = waitHandle;
                queue.BeginPeek(TimeSpan.FromSeconds(1), waitHandle, OnPeekMessage);
            }
        }

        public void Stop()
        {
            ShouldStop = true;
            WaitHandle.WaitAll(waitHandles);
            queue.Close();
        }

        public void Reply(params object[] messages)
        {
            if (currentMessageInformation == null)
                throw new TransactionException("There is no message to reply to, sorry.");

            Send(currentMessageInformation.Source, messages);
        }

        public event Action<CurrentMessageInformation> MessageArrived = delegate { };
        public event Action<CurrentMessageInformation, Exception> MessageProcessingFailure = delegate { };
        public event Action<CurrentMessageInformation> MessageProcessingCompleted = delegate { };

        #endregion

        private void OnPeekMessage(IAsyncResult ar)
        {
            var peek = TryEndingPeek(ar);
            if (peek == false)// error 
                return;

            if (ShouldStop)
            {
                ((ManualResetEvent)ar.AsyncState).Set();
                return;
            }

            if (peek == null)//nothing was found
            {
                queue.BeginPeek(TimeSpan.FromSeconds(1), ar.AsyncState, OnPeekMessage);
                return;
            }

            ReceiveMessageInTransaction();

            queue.BeginPeek(TimeSpan.FromSeconds(1), ar.AsyncState, OnPeekMessage);
        }

        private void ReceiveMessageInTransaction()
        {
            using (var tx = new TransactionScope())
            {
                Message message = null;
                Exception ex = null;
                try
                {
                    queue.MessageReadPropertyFilter.SetAll();
                    message = queue.Receive(
                        TimeSpan.FromSeconds(5),
                        GetTransactionTypeBasedOnQueue(queue));

                    if (DispatchToErrorQueueIfNeeded(message))
                    {
                        return;
                    }

                    ProcessMessage(message);
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    {
                        return;// someone got it before we did, better luck next time
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
                    HandleMessageCompletion(message, tx, ex);
                }

            }
        }

        private bool DispatchToErrorQueueIfNeeded(Message message)
        {
            var id = GetMessageId(message);

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
                message.Extension = Encoding.Unicode.GetBytes(errorCounter.ExceptionText);
                SendMessageToQueue(message, errorEndpoint);
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

            return Transaction.Current == null ?
                MessageQueueTransactionType.Single :
                MessageQueueTransactionType.Automatic;
        }

        private void HandleMessageCompletion(Message message, TransactionScope tx, Exception exception)
        {
            if (exception == null)
            {
                try
                {
                    tx.Complete();
                    RemoveMessageFromFailureTracking(message);
                    return;
                }
                catch (Exception e)
                {
                    logger.Warn("Failed to complete transaction, moving to error mode", e);
                }
            }

            IncrementFailureCount(GetMessageId(message), exception);
            if (queue.Transactional == false)
            {
                queue.Send(message, MessageQueueTransactionType.None);
            }
        }

        private static string GetMessageId(Message message)
        {
            var id = message.Id;
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
            var id = GetMessageId(message);
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

        private bool? TryEndingPeek(IAsyncResult ar)
        {
            try
            {
                queue.EndPeek(ar);
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    logger.Error("Could not peek message from queue", e);
                    return false;
                }
                return null;// nothing found
            }
            return true;
        }

        private void ProcessMessage(Message message)
        {
            var transportMessage = new MsmqTransportMessage(message);
            object[] messages;
            try
            {
                messages = serializer.Deserialize(transportMessage);
            }
            catch (Exception e)
            {
                //serialization exception are not recoverable, move directly
                //to error queue
                message.Extension = Encoding.Unicode.GetBytes(e.ToString());
                SendMessageToQueue(message, errorEndpoint);
                return;
            }
            try
            {
                foreach (var msg in messages)
                {
                    currentMessageInformation = new CurrentMessageInformation
                    {
                        TransportMessage = transportMessage,
                        MessageId = CorrelationId.Parse(message.Id),
                        AllMessages = messages,
                        CorrelationId = CorrelationId.Parse(message.CorrelationId),
                        Message = msg,
                        Source = MsmqUtil.GetQueueUri(message.ResponseQueue)
                    };
                    MessageArrived(currentMessageInformation);
                }
            }
            catch (Exception e)
            {
                try
                {
                    MessageProcessingFailure(currentMessageInformation, e);
                }
                catch(Exception moduleException)
                {
                    throw new TransportException("Module failed to process message failure: " + e.Message,
                                                 moduleException);
                }
                throw;
            }
            finally
            {
                MessageProcessingCompleted(currentMessageInformation);
                currentMessageInformation = null;
            }
        }

        public void Send(Uri uri, params object[] msgs)
        {
            var message = new Message();

            serializer.Serialize(msgs, new MsmqTransportMessage(message));

            message.ResponseQueue = queue;

            SetCorrelationIdOnMessage(message);

            message.Label = msgs
                .Where(msg => msg != null)
                .Select(msg =>
                {
                    var s = msg.ToString();
                    if (s.Length > 249)
                        return s.Substring(0, 246) + "...";
                    return s;
                })
                .FirstOrDefault();

            SendMessageToQueue(message, uri);
        }

        private static void SetCorrelationIdOnMessage(Message message)
        {
            if (currentMessageInformation != null)
                message.CorrelationId = currentMessageInformation.CorrelationId
                    .Increment().ToString();
        }

        private void SendMessageToQueue(Message message, Uri uri)
        {
            var sendQueueDescription = MsmqUtil.GetQueueDescription(uri);
            try
            {
                using (var sendQueue = new MessageQueue(
                    sendQueueDescription.QueuePath,
                    QueueAccessMode.Send))
                {
                    var transactionType = GetTransactionTypeBasedOnQueue(sendQueue);
                    sendQueue.Send(message, transactionType);
                }
            }
            catch (Exception e)
            {
                throw new TransactionException("Failed to send message to " + uri, e);
            }
        }
    }
}