using System.Collections.Generic;
using Rhino.Queues.Storage.InMemory;

namespace Rhino.Queues.Storage.InMemory
{
	public class InMemoryMessageStorageFactory : IStorageFactory
	{
		public IMessageStorage ForOutgoingMessages(HashSet<string> endpoints)
		{
			return new InMemoryMessageStorage(endpoints);
		}

		public IMessageStorage ForIncomingMessages(HashSet<string> endpoints)
		{
			return new InMemoryMessageStorage(endpoints);
		}
	}
}