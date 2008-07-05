using System;

namespace Rhino.Queues
{
	public interface IIncomingMessageRepository
	{
		QueueMessage GetEarliestMessage();

		void Save(params QueueMessage[] msgs);

		void PurgeAllMessages();
	}
}