using System;

namespace Rhino.Queues.Data
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

		void ReturnedFailedBatchToQueue(Guid batchId, Uri destination, int maxFailureCount, Exception exception);
	}
}