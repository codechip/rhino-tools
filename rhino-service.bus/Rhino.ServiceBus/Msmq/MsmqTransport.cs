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
		private bool haveStarted;
		private MessageQueue queue;
		private readonly IQueueStrategy queueStrategy;
		private readonly SortedList<DateTime, string> timeoutMessageIds = new SortedList<DateTime, string>();
		private Timer timeoutTimer;

		private readonly Dictionary<MessageType, Func<QueueState, Message, bool>> actions;

		public MsmqTransport(
			IMessageSerializer serializer,
			Uri endpoint,
			int threadCount,
			int numberOfRetries,
			IQueueStrategy queueStrategy)
		{
			this.serializer = serializer;
			this.endpoint = endpoint;
			this.threadCount = threadCount;
			this.numberOfRetries = numberOfRetries;
			waitHandles = new WaitHandle[threadCount];
			this.queueStrategy = queueStrategy;

			actions = new Dictionary<MessageType, Func<QueueState, Message, bool>>
			{
				{MessageType.DiscardedMessageMarker, MoveToDiscardedQueue},
				{MessageType.AdministrativeMessageMarker, HandleAdministrationMessage},
				{MessageType.ShutDownMessageMarker, ConsumeAndIgnoreMessage},
                {MessageType.ErrorDescriptionMessageMarker, MoveToErrorQueue},
				{MessageType.TimeoutMessageMarker, MoveToTimeoutQueue}
			};
		}


		public volatile bool ShouldStop;

		#region ITransport Members

		public Uri Endpoint
		{
			get { return endpoint; }
		}

		public void Start()
		{
			if (haveStarted)
				return;

			logger.DebugFormat("Starting msmq transport on: {0}", Endpoint);
			queue = InitalizeQueue(endpoint);

			foreach (var message in queueStrategy.GetTimeoutMessages(queue))
			{
				timeoutMessageIds.Add(message.Time, message.Id);
			}

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

			timeoutTimer = new Timer(OnTimeoutCallback, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
			haveStarted = true;
		}

		private static TimeSpan TimeOutForPeek
		{
			get { return TimeSpan.FromHours(1); }
		}

		public void Stop()
		{
			ShouldStop = true;
			queue.Send(new Message
			{
				Label = "Shutdown bus, if you please",
				AppSpecific = (int)MessageType.ShutDownMessageMarker
			}, queue.GetSingleMessageTransactionType());

			WaitForProcessingToEnd();

			if (queue != null)
				queue.Close();
			if (timeoutTimer != null)
				timeoutTimer.Dispose();
			haveStarted = false;
		}

		public void Reply(params object[] messages)
		{
			if (currentMessageInformation == null)
				throw new TransactionException("There is no message to reply to, sorry.");

			Send(currentMessageInformation.Source, messages);
		}

		public event Action<CurrentMessageInformation> MessageSent;
		public event Func<CurrentMessageInformation,bool> AdministrativeMessageArrived;
		public event Action<CurrentMessageInformation> MessageArrived;
		public event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;
		public event Action<CurrentMessageInformation> MessageProcessingCompleted;
        public event Action<CurrentMessageInformation> AdministrativeMessageProcessingCompleted;

		public void Discard(object msg)
		{
			var message = GenerateMsmqMessageFromMessageBatch(new[] { msg });

			message.AppSpecific = (int)MessageType.DiscardedMessageMarker;

			SendMessageToQueue(message, Endpoint);
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
				Source = endpoint,
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

			message.AppSpecific =
				msgs[0] is AdministrativeMessage ? (int)MessageType.AdministrativeMessageMarker : 0;

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

		public event Action<CurrentMessageInformation, Exception> MessageSerializationException;

		#endregion

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

		private static MessageQueue InitalizeQueue(Uri endpoint)
		{
		    try
			{
				var messageQueue = endpoint.CreateQueue(QueueAccessMode.SendAndReceive);
				var filter = new MessagePropertyFilter();
				filter.SetAll();
				messageQueue.MessageReadPropertyFilter = filter;
				return messageQueue;
			}
			catch (Exception e)
			{
				throw new TransportException(
					"Could not open queue: " + endpoint + Environment.NewLine +
					"Queue path: " + MsmqUtil.GetQueuePath(endpoint), e);
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

			try
			{
				if (peek == null)//nothing was found 
					return;

				if (DispatchToErrorQueueIfNeeded(state.Queue, message))
					return;

				Func<QueueState, Message, bool> messageAction;
				if (actions.TryGetValue((MessageType)message.AppSpecific, out messageAction))
				{
					if (messageAction(state, message))
						return;
				}

				logger.DebugFormat("Got message {0} from {1}",
								   message.Label,
								   MsmqUtil.GetQueueUri(state.Queue));

				ReceiveMessageInTransaction(state, message.Id);

			}
			finally
			{
				state.Queue.BeginPeek(TimeOutForPeek, state, OnPeekMessage);
			}
		}

		private static bool ConsumeAndIgnoreMessage(QueueState state, Message message)
		{
			TryGetMessageFromQueue(state.Queue, message.Id);
			return true;
		}

		private void OnTimeoutCallback(object state)
		{
			bool haveTimeoutMessages = false;

			if (readerWriterLock.TryEnterReadLock(0)==false)
				return;
			try
			{
				foreach (var pair in timeoutMessageIds)
				{
					if (pair.Key <= CurrentTime)
					{
						haveTimeoutMessages = true;
						break;
					}
					if (pair.Key > CurrentTime)
					{
						break;
					}
				}
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}

			if (haveTimeoutMessages == false)
				return;

			if (readerWriterLock.TryEnterWriteLock(0)==false)
				return;
			try
			{
				while (timeoutMessageIds.Count != 0)
				{
					var pair = timeoutMessageIds.First();
					if (pair.Key > CurrentTime)
						return;
					timeoutMessageIds.RemoveAt(0);

					try
					{
						using(var tx = new TransactionScope())
						{
							queueStrategy.MoveTimeoutToMainQueue(queue, pair.Value);
							tx.Complete();
						}
					}
					catch (InvalidOperationException)
					{
						//message read by another thread
					}
				}
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		public static DateTime CurrentTime
		{
			get { return DateTime.Now; }
		}

		private bool MoveToDiscardedQueue(QueueState state, Message message)
		{
			queueStrategy.MoveToDiscardedQueue(state.Queue, message);
			return true;
		}

		private bool HandleAdministrationMessage(QueueState state, Message message)
		{
			Exception ex = null;
			try
			{
			    var copy = AdministrativeMessageArrived;
			    Action<CurrentMessageInformation> action = null;
                if (copy != null)
                {
                    action = information =>
                    {
                        var msmqCurrentMessageInformation = (MsmqCurrentMessageInformation) information;
                        var messageProcessedCorrectly = copy(information);

                        if (messageProcessedCorrectly)
                            return;

                        msmqCurrentMessageInformation.Queue.ConsumeMessage(msmqCurrentMessageInformation.MsmqMessage);
                    };
                }

                ProcessMessage(message, state, action, AdministrativeMessageProcessingCompleted);
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

		private void ReceiveMessageInTransaction(QueueState state, string messageId)
		{
			using (var tx = new TransactionScope())
			{
				Message message = null;
				Exception ex = null;
				try
				{
					state.Queue.MessageReadPropertyFilter.SetAll();
					message = TryGetMessageFromQueue(state.Queue, messageId);
					if (message == null)
						return;// someone else ate our message, better luck next time
					ProcessMessage(message, state, MessageArrived, MessageProcessingCompleted);
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

		private static Message TryGetMessageFromQueue(MessageQueue queue, string messageId)
		{
			try
			{
				return queue.ReceiveById(
					messageId,
					queue.GetTransactionType());
			}
			catch (InvalidOperationException)// message was read before we could read it
			{
				return null;
			}
		}

		private bool DispatchToErrorQueueIfNeeded(MessageQueue messageQueue, Message message)
		{
			if (message.AppSpecific != 0)
				return false;

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
				queueStrategy.MoveToErrorsQueue(messageQueue, message);
				var label = "Error description for " + message.Label;
				if (label.Length > 249)
					label = label.Substring(0, 246) + "...";
				messageQueue.Send(new Message
				{
					AppSpecific = (int)MessageType.ErrorDescriptionMessageMarker,
					Label = label,
					Body = errorCounter.ExceptionText,
					CorrelationId = message.Id,
				},messageQueue.GetTransactionType());
				logger.WarnFormat("Moving message {0} to errors subqueue because: {1}", message.Id,
								  errorCounter.ExceptionText);
				return true;
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		private bool MoveToErrorQueue(QueueState state, Message message)
		{
			queueStrategy.MoveToErrorsQueue(state.Queue, message);
			return true;
		}

		private bool MoveToTimeoutQueue(QueueState state, Message message)
		{
			var processMessageAt = DateTime.FromBinary(BitConverter.ToInt64(message.Extension, 0));
			if (CurrentTime >= processMessageAt)
				return false;
			queueStrategy.MoveToTimeoutQueue(state.Queue, message);
			readerWriterLock.EnterWriteLock();
			try
			{
				timeoutMessageIds.Add(processMessageAt, message.Id);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
			return true;
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
			if (state.Queue.Transactional == false)// put the item back in the queue
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

		private void ProcessMessage(Message message, 
            QueueState state, 
            Action<CurrentMessageInformation> messageRecieved,
            Action<CurrentMessageInformation> messageCompleted)
		{
			//deserialization errors do not count for module events
			object[] messages = DeserializeMessages(state, message);
			try
			{
				foreach (object msg in messages)
				{
					currentMessageInformation = new MsmqCurrentMessageInformation
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
                if (messageCompleted != null)
                    messageCompleted(currentMessageInformation);
				currentMessageInformation = null;
			}
		}

		private object[] DeserializeMessages(QueueState state, Message transportMessage)
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
						var information = new CurrentMessageInformation
						{
							Message = transportMessage,
							Source = MsmqUtil.GetQueueUri(state.Queue),
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
			if (currentMessageInformation != null)
				message.CorrelationId = currentMessageInformation.CorrelationId
					.Increment().ToString();
		}

		private void SendMessageToQueue(Message message, Uri uri)
		{
			if (haveStarted == false)
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
