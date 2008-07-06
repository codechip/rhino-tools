using System;
using System.Threading;
using System.Transactions;
using Rhino.Queues.Data;
using Rhino.Queues.Workers;

namespace Rhino.Queues.Impl
{
	public class Queue : IQueueImpl, IEnlistmentNotification
	{
		private readonly IIncomingMessageRepository incomingMessageRepository;
		private readonly IOutgoingMessageRepository outgoingMessageRepository;
		private readonly AutoResetEvent waitForItemInQueue = new AutoResetEvent(false);
		private QueueType type;

		public Queue(
			Uri url,
			IOutgoingMessageRepository outgoingMessageRepository,
			IIncomingMessageRepository incomingMessageRepository,
			QueueType queueType)
		{
			Url = url;
			this.outgoingMessageRepository = outgoingMessageRepository;
			this.incomingMessageRepository = incomingMessageRepository;
			type = queueType;
		}

		public Uri Url { get; private set; }

		#region IEnlistmentNotification Members

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Commit(Enlistment enlistment)
		{
			enlistment.Done();
			OnMessageArrived();
			waitForItemInQueue.Set();
		}

		public void Rollback(Enlistment enlistment)
		{
			enlistment.Done();
		}

		public void InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}

		#endregion

		public void OnMessageArrived()
		{
			Action<QueueMessage> messageArrived = MessageArrived;
			if (messageArrived == null)
				return;
			ThreadPool.QueueUserWorkItem(delegate
			{
				var message = incomingMessageRepository.GetEarliestMessage();
				while (message != null)
				{
					var loopCopy = message;

					messageArrived(loopCopy);
					message = incomingMessageRepository.GetEarliestMessage();
				}
			});
		}

		#region IQueueImpl Members

		public void AcceptMessages(params QueueMessage[] msgs)
		{
			incomingMessageRepository.Save(msgs);

			if (Transaction.Current == null)
			{
				OnMessageArrived();
				waitForItemInQueue.Set();
			}
			else
			{
				Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
			}
		}

		public void FailedToTransfer(SingleDestinationMessageBatch batch, Exception e)
		{
			outgoingMessageRepository.ReturnedFailedBatchToQueue(batch.BatchId, batch.Destination, 100, e);
		}

		public void SuccessfullyTransfered(SingleDestinationMessageBatch batch)
		{
			outgoingMessageRepository.RemoveSuccessfulBatch(batch.BatchId, batch.Destination);
		}

		public void PurgeAllMessages()
		{
			if (type == QueueType.Input)
				incomingMessageRepository.PurgeAllMessages();
			else
				outgoingMessageRepository.PurgeAllMessages();
		}

		public QueueMessage Recieve()
		{
			return Recieve(TimeSpan.FromMinutes(1));
		}

		public QueueMessage Recieve(TimeSpan timeout)
		{
			var message = incomingMessageRepository.GetEarliestMessage();
			while (message == null && timeout > TimeSpan.Zero)
			{
				DateTime startWaiting = DateTime.Now;
				WaitForNewMessageToArrive(timeout);
				message = incomingMessageRepository.GetEarliestMessage();
				TimeSpan durationWaited = DateTime.Now - startWaiting;
				timeout -= durationWaited;
			}
			return message;
		}

		public event Action<QueueMessage> MessageArrived;

		public void Send(QueueMessage message)
		{
			message.Id = SequentialGuid.Next();
			outgoingMessageRepository.Save(Url, message);
		}

		#endregion

		public bool WaitForNewMessageToArrive(TimeSpan timeout)
		{
			return waitForItemInQueue.WaitOne(timeout, false);
		}
	}
}