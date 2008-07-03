using System;

namespace Rhino.Queues
{
	public interface IIncomingMessageRepository
	{
		QueueMessage GetEarliestMessage();

		void Save(QueueMessage msg);

		void Transaction(Action action);

		void PurgeAllMessages();

		void CreateQueueStorage();
	}
}