using System;
using System.Messaging;
using System.Transactions;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus.MessageModules
{
    public class MessageLoggingModule : IMessageModule
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly Uri logQueue;
        private MessageQueue queue;
        private bool transactional;

        public MessageLoggingModule(IMessageSerializer messageSerializer, Uri logQueue)
        {
            this.messageSerializer = messageSerializer;
            this.logQueue = logQueue;
        }

        public void Init(ITransport transport)
        {
            queue = logQueue.CreateQueue(QueueAccessMode.Send);
            transactional = queue.Transactional;

            transport.MessageArrived += Transport_OnMessageArrived;
            transport.MessageProcessingFailure += Transport_OnMessageProcessingFailure;
            transport.MessageProcessingCompleted += Transport_OnMessageProcessingCompleted;
            transport.MessageSerializationException += Transport_OnMessageSerializationException;
        }

        public void Stop(ITransport transport)
        {
            transport.MessageArrived -= Transport_OnMessageArrived;
            transport.MessageProcessingFailure -= Transport_OnMessageProcessingFailure;
            transport.MessageProcessingCompleted -= Transport_OnMessageProcessingCompleted;
            transport.MessageSerializationException -= Transport_OnMessageSerializationException;

            queue.Dispose();
        }

        private void Send(object obj)
        {
            Send(obj, GetTransactionType());
        }

        private void Send(object obj, MessageQueueTransactionType transactionType)
        {
            var msg = new Message();
            messageSerializer.Serialize(new[] { obj }, msg.BodyStream);
            queue.Send(msg, obj.ToString(), transactionType);
        }

        private MessageQueueTransactionType GetTransactionType()
        {
            if (transactional)
            {
                if (Transaction.Current != null)
                    return MessageQueueTransactionType.Automatic;
                return MessageQueueTransactionType.Single;
            }
            return MessageQueueTransactionType.None;
        }

        private void Transport_OnMessageSerializationException(CurrentMessageInformation info, Exception t)
        {
            Send(new SerializationErrorMessage
            {
                MessageId = info.MessageId,
                CorrelationId = info.CorrelationId,
                Error = t.ToString(),
                Source = info.Source,
            });
        }

        private void Transport_OnMessageProcessingCompleted(CurrentMessageInformation info)
        {
            Send(new MessageProcessingCompletedMessage
            {
                Timestamp = DateTime.Now,
                MessageType = info.Message.ToString(),
                MessageId = info.MessageId,
                CorrelationId = info.CorrelationId,
                Source = info.Source,
            });
        }

        internal void Transport_OnMessageProcessingFailure(CurrentMessageInformation info, Exception e)
        {
            Send(new MessageProcessingFailedMessage
            {
                ErrorText = e.ToString(),
                Timestamp = DateTime.Now,
                MessageType = info.Message.ToString(),
                MessageId = info.MessageId,
                CorrelationId = info.CorrelationId,
                Source = info.Source,
                Message = info.Message
            }, transactional ? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
        }

        private void Transport_OnMessageArrived(CurrentMessageInformation info)
        {
            Send(new MessageArrivedMessage
            {
                Timestamp = DateTime.Now,
                MessageType = info.Message.ToString(),
                MessageId = info.MessageId,
                CorrelationId = info.CorrelationId,
                Source = info.Source,
                Message = info.Message
            });
        }
    }
}