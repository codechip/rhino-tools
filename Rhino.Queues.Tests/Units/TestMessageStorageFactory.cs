using System.Collections.Generic;
using Rhino.Queues.Storage;
using Rhino.Queues.Storage.InMemory;

namespace Rhino.Queues.Tests.Units
{
	public class TestMessageStorageFactory : IStorageFactory
	{
		public InMemoryMessageStorage OutgoingStorage { get; private set; }
		public InMemoryMessageStorage IncomingStorage { get; private set; }

		public IMessageStorage ForOutgoingMessages(HashSet<string> endpoints)
		{
			OutgoingStorage = new InMemoryMessageStorage(endpoints);
			return OutgoingStorage;
		}

		public IMessageStorage ForIncomingMessages(HashSet<string> endpoints)
		{
			IncomingStorage = new InMemoryMessageStorage(endpoints);
			return IncomingStorage;
		}
	}
}