namespace Rhino.Queues.Tests.Storage.Disk
{
	using System;
	using System.Collections.Generic;
	using System.Transactions;
	using MbUnit.Framework;
	using Rhino.Queues.Storage.Disk;

	[TestFixture]
	public class PerformanceTests : PersistentQueueTestsBase
	{
		[Test]
		public void Enqueue_million_items_with_thousand_flushes()
		{
			using (var queue = new PersistentQueue(path))
			{
				for (int i = 0; i < 1000; i++)
				{
					using (var session = queue.OpenSession())
					using (var tx = new TransactionScope())
					{
						for (int j = 0; j < 1000; j++)
						{
							session.Enqueue(Guid.NewGuid().ToByteArray());
						}
						tx.Complete();
					}
				}
			}
		}

		[Test]
		public void Enqueue_million_items_with_single_flush()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < largeCount; i++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}
			}
		}

		[Test]
		public void Enqueue_and_dequeue_million_items_same_queue()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < largeCount; i++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}
			
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < largeCount; i++)
					{
						new Guid(session.Dequeue());
					}
					tx.Complete();
				}
			}
		}

		[Test]
		public void Enqueue_and_dequeue_million_items_restart_queue()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < largeCount; i++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}
			}

			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < largeCount; i++)
					{
						new Guid(session.Dequeue());
					}
					tx.Complete();
				}
			}
		}

		[Test]
		public void Enqueue_and_dequeue_large_items_with_restart_queue()
		{
			var random = new Random();
			var itemsSizes = new List<int>();
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < smallCount; i++)
					{
						var data = new byte[random.Next(1024*512, 1024*1024)];
						itemsSizes.Add(data.Length);
						session.Enqueue(data);
					}
					tx.Complete();
				}
			}

			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int i = 0; i < smallCount; i++)
					{
						Assert.AreEqual(itemsSizes[i], session.Dequeue().Length);
					}
					tx.Complete();
				}
			}
		}

		private const int largeCount = 1000000;
		private const int smallCount = 500;

	}
}