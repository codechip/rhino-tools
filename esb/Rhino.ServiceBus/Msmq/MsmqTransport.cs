using System;
using System.Collections.Generic;
using System.Messaging;
using System.Runtime.Serialization;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq.TransportActions;
using Rhino.ServiceBus.Transport;
using MessageType=Rhino.ServiceBus.Transport.MessageType;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqTransport : AbstractMsmqListener, IMsmqTransport
	{
		[ThreadStatic]
		private static MsmqCurrentMessageInformation currentMessageInformation;

        public static MsmqCurrentMessageInformation CurrentMessageInformation
        {
            get { return currentMessageInformation; }
        }

        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqTransport));
        private readonly IMsmqTransportAction[] transportActions;
    	private readonly IsolationLevel queueIsolationLevel;

    	public MsmqTransport(IMessageSerializer serializer, IQueueStrategy queueStrategy, Uri endpoint, int threadCount, IMsmqTransportAction[] transportActions, IEndpointRouter endpointRouter, IsolationLevel queueIsolationLevel)
            :base(queueStrategy,endpoint, threadCount, serializer,endpointRouter)
        {
        	this.transportActions = transportActions;
        	this.queueIsolationLevel = queueIsolationLevel;
        }

    	#region ITransport Members

        protected override void BeforeStart(OpenedQueue queue)
        {
            foreach (var messageAction in transportActions)
            {
                messageAction.Init(this, queue);
            }
        }

        public void Reply(params object[] messages)
		{
			if (currentMessageInformation == null)
				throw new TransactionException("There is no message to reply to, sorry.");
            logger.DebugFormat("Replying to {0}", currentMessageInformation.Source);
            Send(endpointRouter.GetRoutedEndpoint(currentMessageInformation.Source), messages);
		}

        public event Action<CurrentMessageInformation> MessageSent;
	    
        public event Func<CurrentMessageInformation, bool> AdministrativeMessageArrived;
		
        public event Func<CurrentMessageInformation, bool> MessageArrived;
		
        public event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;
        
        public event Action<CurrentMessageInformation, Exception> MessageProcessingCompleted;
        
        public event Action<CurrentMessageInformation, Exception> AdministrativeMessageProcessingCompleted;

		public void Discard(object msg)
		{
			var message = GenerateMsmqMessageFromMessageBatch(new[] { msg });

            SendMessageToQueue(message.SetSubQueueToSendTo(SubQueue.Discarded), Endpoint);
		}

	    public bool RaiseAdministrativeMessageArrived(CurrentMessageInformation information)
	    {
            var copy = AdministrativeMessageArrived;
            if (copy != null)
                return copy(information);
	        return false;
        }

	    public void RaiseAdministrativeMessageProcessingCompleted(CurrentMessageInformation information, Exception ex)
	    {
	        var copy = AdministrativeMessageProcessingCompleted;
            if (copy != null)
                copy(information, ex);
	    }

	    public void Send(Endpoint endpoint, DateTime processAgainAt, object[] msgs)
		{
			if (HaveStarted == false)
				throw new InvalidOperationException("Cannot send a message before transport is started");
			
			var message = GenerateMsmqMessageFromMessageBatch(msgs);
	        var bytes = new List<byte>(message.Extension);
	        bytes.AddRange(BitConverter.GetBytes(processAgainAt.ToBinary()));
	        message.Extension = bytes.ToArray();
			message.AppSpecific = (int)MessageType.TimeoutMessageMarker;

            SendMessageToQueue(message, endpoint);
		}

        public void Send(Endpoint destination, object[] msgs)
		{
			if(HaveStarted==false)
				throw new InvalidOperationException("Cannot send a message before transport is started");

			var message = GenerateMsmqMessageFromMessageBatch(msgs);

            SendMessageToQueue(message, destination);

			var copy = MessageSent;
			if (copy == null)
				return;

			copy(new CurrentMessageInformation
			{
				AllMessages = msgs,
				Source = Endpoint.Uri,
				Destination = destination.Uri,
                MessageId = message.GetMessageId(),
			});
		}

		public event Action<CurrentMessageInformation, Exception> MessageSerializationException;

		#endregion

        public void ReceiveMessageInTransaction(OpenedQueue queue, string messageId, Func<CurrentMessageInformation, bool> messageArrived, Action<CurrentMessageInformation, Exception> messageProcessingCompleted)
		{
        	var transactionOptions = new TransactionOptions
        	{
				IsolationLevel = queueIsolationLevel,
				Timeout = TransportUtil.GetTransactionTimeout(),
        	};
			using (var tx = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				var message = queue.TryGetMessageFromQueue(messageId);
                
                if (message == null)
                    return;// someone else got our message, better luck next time

                ProcessMessage(message, queue, tx, messageArrived, messageProcessingCompleted);
			}
		}

        public void RaiseMessageSerializationException(OpenedQueue queue, Message msg, string errorMessage)
        {
            var copy = MessageSerializationException;
            if (copy == null)
                return;
            var messageInformation = new MsmqCurrentMessageInformation
            {
                MsmqMessage = msg,
                Queue = queue,
                Message = null,
                Source = queue.RootUri,
                MessageId = Guid.Empty
            };
            copy(messageInformation, new SerializationException(errorMessage));
        }

        public OpenedQueue CreateQueue()
        {
            return InitalizeQueue(Endpoint);
        }

        private void HandleMessageCompletion(
			Message message,
			TransactionScope tx,
            OpenedQueue messageQueue,
			Exception exception)
		{
			if (exception == null)
			{
				try
				{
					if (tx != null)
						tx.Complete();
					return;
				}
				catch (Exception e)
				{
					logger.Warn("Failed to complete transaction, moving to error mode", e);
				}
			}
			if (message == null)
				return;

            try
            {
                Action<CurrentMessageInformation, Exception> copy = MessageProcessingFailure;
                if (copy != null)
                    copy(currentMessageInformation, exception);
            }
            catch (Exception moduleException)
            {
                string exMsg = "";
                if (exception != null)
                    exMsg = exception.Message;
                logger.Error("Module failed to process message failure: " + exMsg,
                                             moduleException);
            }

            if (messageQueue.IsTransactional == false)// put the item back in the queue
			{
                messageQueue.Send(message);
			}
		}

        private void ProcessMessage(
			Message message, 
            OpenedQueue messageQueue, 
            TransactionScope tx,
            Func<CurrentMessageInformation, bool> messageRecieved,
            Action<CurrentMessageInformation, Exception> messageCompleted)
		{
		    Exception ex = null;
		    currentMessageInformation = CreateMessageInformation(messageQueue, message, null, null);
            try
            {
                //deserialization errors do not count for module events
                object[] messages = DeserializeMessages(messageQueue, message, MessageSerializationException);
                try
                {
                    foreach (object msg in messages)
                    {
                        currentMessageInformation = CreateMessageInformation(messageQueue,message, messages, msg);

                        if(TransportUtil.ProcessSingleMessage(currentMessageInformation,messageRecieved)==false)
                            Discard(currentMessageInformation.Message);
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                    logger.Error("Failed to process message", e);
                }
                finally
                {
                    try
                    {
                        if (messageCompleted != null)
                            messageCompleted(currentMessageInformation, ex);
                    }
                    catch (Exception e)
                    {
                        logger.Error("An error occured when raising the MessageCompleted event, the error will NOT affect the message processing", e);
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                logger.Error("Failed to deserialize message", e);
            }
            finally
		    {
                HandleMessageCompletion(message, tx, messageQueue, ex);
                currentMessageInformation = null;
		    } 
		}

	    private MsmqCurrentMessageInformation CreateMessageInformation(OpenedQueue queue,Message message, object[] messages, object msg)
	    {
	        return new MsmqCurrentMessageInformation
	        {
                MessageId = message.GetMessageId(),
	            AllMessages = messages,
	            Message = msg,
	            Queue = queue,
                TransportMessageId = message.Id,
	            Destination = Endpoint.Uri,
	            Source = MsmqUtil.GetQueueUri(message.ResponseQueue),
	            MsmqMessage = message,
	            TransactionType = queue.GetTransactionType()
	        };
	    }

        private void SendMessageToQueue(Message message, Endpoint endpoint)
		{
			if (HaveStarted == false)
				throw new TransportException("Cannot send message before transport is started");

            try
			{
				using (var sendQueue = MsmqUtil.GetQueuePath(endpoint).Open(QueueAccessMode.Send))
				{
					sendQueue.Send(message);
					logger.DebugFormat("Send message {0} to {1}", message.Label, endpoint);
				}
			}
			catch (Exception e)
			{
				throw new TransactionException("Failed to send message to " + endpoint, e);
			}
		}

        protected override void HandlePeekedMessage(OpenedQueue queue,Message message)
        {
            foreach (var action in transportActions)
            {
                if(action.CanHandlePeekedMessage(message)==false)
                    continue;

                try
                {
                    if (action.HandlePeekedMessage(this, queue, message))
                        return;
                }
                catch (Exception e)
                {
                    logger.Error("Error when trying to execute action " + action + " on message " + message.Id + ". Message has been removed without handling!", e);
                    queue.ConsumeMessage(message.Id);
                }
            }

            ReceiveMessageInTransaction(queue, message.Id, MessageArrived, MessageProcessingCompleted);
        }
	}
}
