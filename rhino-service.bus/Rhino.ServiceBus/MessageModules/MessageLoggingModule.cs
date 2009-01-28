using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus.MessageModules
{
    public class MessageLoggingModule : IMessageModule
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly IEndpointRouter endpointRouter;
        private readonly Uri logQueue;
        private MessageQueue queue;

        public MessageLoggingModule(IMessageSerializer messageSerializer, IEndpointRouter endpointRouter, Uri logQueue)
        {
            this.messageSerializer = messageSerializer;
            this.endpointRouter = endpointRouter;
            this.logQueue = logQueue;
        }

        public void Init(ITransport transport)
        {
            queue = endpointRouter.GetRoutedEndpoint(logQueue).CreateQueue(QueueAccessMode.Send);

            transport.MessageArrived += Transport_OnMessageArrived;
            transport.MessageProcessingFailure += Transport_OnMessageProcessingFailure;
            transport.MessageProcessingCompleted += Transport_OnMessageProcessingCompleted;
            transport.MessageSerializationException += Transport_OnMessageSerializationException;
            transport.MessageSent+=Transport_OnMessageSent;
        }

        public void Stop(ITransport transport)
        {
            transport.MessageArrived -= Transport_OnMessageArrived;
            transport.MessageProcessingFailure -= Transport_OnMessageProcessingFailure;
            transport.MessageProcessingCompleted -= Transport_OnMessageProcessingCompleted;
            transport.MessageSerializationException -= Transport_OnMessageSerializationException;
            transport.MessageSent -= Transport_OnMessageSent;

            queue.Dispose();
        }

        private void Send(object obj)
        {
            Send(obj, queue.GetTransactionType());
        }

        private void Transport_OnMessageSent(CurrentMessageInformation info)
        {
            Send(new MessageSentMessage
            {
                MessageId = info.MessageId,
                Source = info.Source,
                Message = info.AllMessages,
                MessageType = info.AllMessages[0].ToString(),
                Timestamp = DateTime.Now,
                Destination = info.Destination
            });
        }

        private void Send(object obj, MessageQueueTransactionType transactionType)
        {
            var msg = new Message();
            messageSerializer.Serialize(new[] { obj }, msg.BodyStream);
            queue.Send(msg, obj.ToString(), transactionType);
        }

        private void Transport_OnMessageSerializationException(CurrentMessageInformation info, Exception t)
        {
            Send(new SerializationErrorMessage
            {
                MessageId = info.MessageId,
                Error = t.ToString(),
                Source = info.Source,
            });
        }

         private void Transport_OnMessageProcessingCompleted(CurrentMessageInformation info, Exception ex)
        {
            Send(new MessageProcessingCompletedMessage
            {
                Timestamp = DateTime.Now,
                MessageType = info.Message.ToString(),
                MessageId = info.MessageId,
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
                Source = info.Source,
                Message = info.Message
            }, queue.GetSingleMessageTransactionType());
        }

        private bool Transport_OnMessageArrived(CurrentMessageInformation info)
        {
            Send(new MessageArrivedMessage
            {
                Timestamp = DateTime.Now,
                MessageType = info.Message.ToString(),
                MessageId = info.MessageId,
                Source = info.Source,
                Message = info.Message
            });
            return false;
        }
    }
}