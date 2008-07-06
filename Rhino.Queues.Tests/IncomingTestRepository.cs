using System;
using BerkeleyDb;

namespace Rhino.Queues.Tests
{
	public class IncomingTestRepository : IDisposable
	{
		private readonly string name;

		public IncomingTestRepository(string name)
		{
			this.name = name;
		}

		public QueueMessage GetLatestMessage()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				return (QueueMessage)queue.Consume();
			}
		}

		public void Dispose()
		{
		}
	}
}