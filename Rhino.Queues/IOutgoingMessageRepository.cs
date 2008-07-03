using System;

namespace Rhino.Queues
{
	public interface IOutgoingMessageRepository 
	{
		string Name { get;  }
		
		event Action NewMessageStored;

		void Save(Uri destination, QueueMessage msg);

		void PurgeAllMessages();

		MessageBatch GetBatchOfMessagesToSend();

		void ResetAllBatches();

		void RemoveSuccessfulBatch(Guid batchId, Uri destination);

		void MarkAllInBatchAsFailed(Guid batchId, Uri destination);

		void ResetBatch(Guid batchId, Uri destination);

		void MoveUnderliverableMessagesToDeadLetterQueue(
			Guid batchId,
			Uri destination,
			int minNumberOfFailures,
			Exception lastException);

		void Transaction(Action action);

		void CreateQueueStorage();
	}
}