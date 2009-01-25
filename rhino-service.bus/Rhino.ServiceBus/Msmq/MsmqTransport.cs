using System;
using System.Linq;
using System.Messaging;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq.TransportActions;

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
		private readonly IMessageSerializer serializer;
        private readonly ITransportAction[] transportActions;

		public MsmqTransport(IMessageSerializer serializer, Uri endpoint, int threadCount, ITransportAction[] transportActions)
            :base(endpoint, threadCount)
		{
			this.serializer = serializer;
		    this.transportActions = transportActions;
		}

        #region ITransport Members

        protected override void BeforeStart()
        {
            foreach (var messageAction in transportActions)
            {
                messageAction.Init(this);
            }
        }

        public void Reply(params object[] messages)
		{
			if (currentMessageInformation == null)
				throw new TransactionException("There is no message to reply to, sorry.");

			Send(currentMessageInformation.Source, messages);
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

			message.AppSpecific = (int)MessageType.DiscardedMessageMarker;

			SendMessageToQueue(message, Endpoint);
		}

	    public bool RaiseAdministrativeMessageArrived(CurrentMessageInformation information)
	    {
            var copy = AdministrativeMessageArrived;
            if (copy != null)
                return copy(information);
	        return false;
        }

	    public MessageQueue Queue
	    {
	        get { return queue; }
	    }

	    public void RaiseAdministrativeMessageProcessingCompleted(CurrentMessageInformation information, Exception ex)
	    {
	        var copy = AdministrativeMessageProcessingCompleted;
            if (copy != null)
                copy(information, ex);
	    }

	    public void Send(Uri uri, DateTime processAgainAt, object[] msgs)
		{
			var message = GenerateMsmqMessageFromMessageBatch(msgs);

			message.Extension = BitConverter.GetBytes(processAgainAt.ToBinary());
			message.AppSpecific = (int)MessageType.TimeoutMessageMarker;

			SendMessageToQueue(message, uri);
		}

        public void Send(Uri uri, params object[] msgs)
		{
			var message = GenerateMsmqMessageFromMessageBatch(msgs);

			SendMessageToQueue(message, uri);

			var copy = MessageSent;
			if (copy == null)
				return;

			copy(new CurrentMessageInformation
			{
				AllMessages = msgs,
				Source = Endpoint,
				Destination = uri,
                CorrelationId = CorrelationId.Parse(message.CorrelationId),
                MessageId = CorrelationId.Parse(message.Id),
			});
		}

		private Message GenerateMsmqMessageFromMessageBatch(object[] msgs)
		{
			var message = new Message();

			serializer.Serialize(msgs, message.BodyStream);

			message.ResponseQueue = queue;

			SetCorrelationIdOnMessage(message);

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

        private static int GetAppSpecificMarker(object[] msgs)
        {
            var msg = msgs[0];
            if (msg is AdministrativeMessage)
                return (int) MessageType.AdministrativeMessageMarker;
            //if (msg is LoadBalancerMessage)
            //    return (int) MessageType.LoadBalancerMessage;
            return 0;
        }

        public event Action<CurrentMessageInformation, Exception> MessageSerializationException;

		#endregion

        protected void ReceiveMessageInTransaction(QueueState state, string messageId)
		{
			using (var tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionTimeout()))
			{
				Message message = state.Queue.TryGetMessageFromQueue(messageId);
                
                if (message == null)
                    return;// someone else got our message, better luck next time

                ProcessMessage(message, state.Queue, tx, MessageArrived, MessageProcessingCompleted);
			}
		}

        private void HandleMessageCompletion(
			Message message,
			TransactionScope tx,
            MessageQueue messageQueue,
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

            if (messageQueue.Transactional == false)// put the item back in the queue
			{
                messageQueue.Send(message, MessageQueueTransactionType.None);
			}
		}

        public void ProcessMessage(Message message, 
            MessageQueue messageQueue, 
            TransactionScope tx,
            Func<CurrentMessageInformation, bool> messageRecieved,
            Action<CurrentMessageInformation, Exception> messageCompleted)
		{
		    Exception ex = null;
		    currentMessageInformation = CreateMessageInformation(message, null, null);
            try
            {
                //deserialization errors do not count for module events
                object[] messages = DeserializeMessages(messageQueue, message);
                try
                {
                    foreach (object msg in messages)
                    {
                        currentMessageInformation = CreateMessageInformation(message, messages, msg);

                        if(ProcessSingleMessage(messageRecieved)==false)
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
                    if (messageCompleted != null)
                        messageCompleted(currentMessageInformation, ex);
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

	    private static bool ProcessSingleMessage(Func<CurrentMessageInformation, bool> messageRecieved)
	    {
	        if (messageRecieved == null)
	            return false;
	        foreach (Func<CurrentMessageInformation, bool> func in messageRecieved.GetInvocationList())
	        {
	            if (func(currentMessageInformation))
	            {
	                return true;
	            }
	        }
	        return false;
	    }

	    private MsmqCurrentMessageInformation CreateMessageInformation(Message message, object[] messages, object msg)
	    {
	        return new MsmqCurrentMessageInformation
	        {
                MessageId = CorrelationId.Parse(message.Id),
	            AllMessages = messages,
                CorrelationId = CorrelationId.Parse(message.CorrelationId),
	            Message = msg,
	            Queue = queue,
	            Destination = Endpoint,
	            Source = MsmqUtil.GetQueueUri(message.ResponseQueue),
	            MsmqMessage = message,
	            TransactionType = queue.GetTransactionType()
	        };
	    }

        private object[] DeserializeMessages(MessageQueue messageQueue, Message transportMessage)
		{
			object[] messages;
			try
			{
				messages = serializer.Deserialize(transportMessage.BodyStream);
			}
			catch (Exception e)
			{
				try
				{
					logger.Error("Error when serializing message", e);
					Action<CurrentMessageInformation, Exception> copy = MessageSerializationException;
					if (copy != null)
					{
						var information = new MsmqCurrentMessageInformation
						{
                            MsmqMessage = transportMessage,
                            Queue = messageQueue,
                            CorrelationId = CorrelationId.Parse(transportMessage.CorrelationId),
							Message = transportMessage,
                            Source = MsmqUtil.GetQueueUri(messageQueue),
                            MessageId = CorrelationId.Parse(transportMessage.Id)
						};
						copy(information, e);
					}
				}
				catch (Exception moduleEx)
				{
					logger.Error("Error when notifying about serialization exception", moduleEx);
				}
				throw;
			}
			return messages;
		}

		private static void SetCorrelationIdOnMessage(Message message)
		{
		    if (currentMessageInformation == null) 
                return;

		    message.CorrelationId = currentMessageInformation.CorrelationId ??
		                            currentMessageInformation.MessageId;
		}

	    private void SendMessageToQueue(Message message, Uri uri)
		{
			if (HaveStarted == false)
				throw new TransportException("Cannot send message before transport is started");

			string sendQueueDescription = MsmqUtil.GetQueuePath(uri);
			try
			{
				using (var sendQueue = new MessageQueue(
					sendQueueDescription,
					QueueAccessMode.Send))
				{
					MessageQueueTransactionType transactionType = sendQueue.GetTransactionType();
					sendQueue.Send(message, transactionType);
					logger.DebugFormat("Send message {0} to {1}", message.Label, uri);
				}
			}
			catch (Exception e)
			{
				throw new TransactionException("Failed to send message to " + uri, e);
			}
		}

        protected override void HandlePeekedMessage(QueueState state, Message message)
        {
            foreach (var action in transportActions)
            {
                if(action.CanHandlePeekedMessage(message)==false)
                    continue;

                if (action.HandlePeekedMessage(queue, message))
                    return;
            }

            ReceiveMessageInTransaction(state, message.Id);
        }
	}
}
