using System;

namespace Rhino.Queues.Data
{
	public interface IIncomingMessageRepository
	{
		QueueMessage GetEarliestMessage();

		void Save(params QueueMessage[] msgs);

		void PurgeAllMessages();
	}
}