using System.Collections.Generic;

namespace Rhino.Queues.Storage
{
	public interface IStorageFactory
	{
		IMessageStorage ForOutgoingMessages(HashSet<string> endpoints);
		IMessageStorage ForIncomingMessages(HashSet<string> inMemoryQueues, HashSet<string> durableQueues);
	}
}