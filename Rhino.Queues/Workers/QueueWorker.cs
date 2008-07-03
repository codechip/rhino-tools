using System;
using System.Threading;
using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Workers
{
	public class QueueWorker : IQueueWorker
	{
		private readonly ICommandFactory commandFactory;
		private readonly IQueueFactoryImpl queueFactory;
		private readonly IOutgoingMessageRepository outgoingMessageRepository;
		private readonly ManualResetEvent checkForMessages = new ManualResetEvent(true);
		private bool shouldStop;

		public event Action Waiting = delegate { };
		public event Action Running = delegate { };

		public QueueWorker(
			ICommandFactory commandFactory,
			IQueueFactoryImpl queueFactory,
			IOutgoingMessageRepository outgoingMessageRepository)
		{
			this.commandFactory = commandFactory;
			this.queueFactory = queueFactory;
			this.outgoingMessageRepository = outgoingMessageRepository;
		}

		public void Stop()
		{
			shouldStop = true;
			checkForMessages.Set();
		}

		public void Run()
		{
			while (shouldStop == false)
			{
				Running();
				MessageBatch messageBatch;
				do // as long as there are messages we don't want to wait
				{
					messageBatch = outgoingMessageRepository.GetBatchOfMessagesToSend();
					foreach (var batch in messageBatch.DestinationBatches)
					{
						batch.Source = queueFactory.LocalUri;
						foreach (var message in batch.Messages)
						{
							message.Source = queueFactory.LocalUri;
						}
						ICommand cmd = commandFactory.CreateCommand(batch);
						cmd.Execute();
					}
				} while (messageBatch.IsEmpty == false && shouldStop == false);
				if(shouldStop)
					return;//skip the waiting cycle
				checkForMessages.Reset();// so we would wait
				Waiting();
				checkForMessages.WaitOne(TimeToWait(), false);
			}
		}

		public void NewMessageStored()
		{
			checkForMessages.Set();
		}

		public Func<TimeSpan> TimeToWait = () => TimeSpan.FromSeconds(1);
	}
}