using System;
using System.Collections.Generic;
using System.Linq;
using BerkeleyDb;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	public class OutgoingTestRepository : IDisposable
	{
		private readonly string name;

		public OutgoingTestRepository(string name)
		{
			this.name = name;
		}

		public QueueTransportMessage[] GetTransportMessages()
		{
			var msgs = new List<QueueTransportMessage>();
			using (var env = new BerkeleyDbEnvironment(name))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				foreach (var message in queue.SelectFromAssociation<QueueTransportMessage>(tree))
				{
					msgs.Add(message);
				}
				tx.Commit();
			}
			return msgs.ToArray();
		}

		public FailedQueueMessage[] GetDeadLetters()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var dead = env.OpenTree(name + ".deadLetters"))
			{
				return dead.Select().Select(x=>x.Value)
					.Cast<FailedQueueMessage>().ToArray();
			}
		}

		public int GetCountInBatches()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var batches = env.OpenTree(name + ".batches"))
			{
				return batches.Select().Count();
			}
		}

		public int GetCountInDeadLetters()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var dead = env.OpenTree(name + ".deadLetters"))
			{
				return dead.Select().Count();
			}
		}

		public int GetCountInActiveMessages()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				return queue.Select().Cast<Guid>().Count();
			}
		}

		public void Dispose()
		{
		}

		public QueueTransportMessage GetLatestMessage()
		{
			using (var env = new BerkeleyDbEnvironment(name))
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				return (QueueTransportMessage)tree.Get(queue.Consume());
				
			}
		}
	}
}