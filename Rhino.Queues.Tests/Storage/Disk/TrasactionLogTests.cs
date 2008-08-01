namespace Rhino.Queues.Tests.Storage.Disk
{
	using System;
	using System.IO;
	using System.Transactions;
	using MbUnit.Framework;
	using Rhino.Queues.Storage.Disk;

	[TestFixture]
	public class TrasactionLogTests : PersistentQueueTestsBase
	{
		[Test]
		public void Transaction_log_size_shrink_after_queue_disposed()
		{
			long txSizeWhenOpen;
			var txLogInfo = new FileInfo(Path.Combine(path, "transaction.log"));
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}

				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Dequeue();
					}
					tx.Complete();
				}
				txSizeWhenOpen = txLogInfo.Length;
			}
			txLogInfo.Refresh();
			Assert.Less(txLogInfo.Length, txSizeWhenOpen);
		}

		[Test]
		public void Count_of_items_will_remain_fixed_after_dequeqing_without_flushing()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}

				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Dequeue();
					}
					Assert.IsNull(session.Dequeue());

					//	tx.Complete(); explicitly removed
				}
			}
			using (var queue = new PersistentQueue(path))
			{
				Assert.AreEqual(10, queue.EstimatedCountOfItemsInQueue);
			}
		}

		[Test]
		public void Dequeue_items_that_were_not_flushed_will_appear_after_queue_restart()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}

				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Dequeue();
					}
					Assert.IsNull(session.Dequeue());

					//	tx.Complete(); explicitly removed
				}
			}
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 10; j++)
					{
						session.Dequeue();
					}
					Assert.IsNull(session.Dequeue());
					tx.Complete();
				}
			}
		}

		[Test]
		public void If_tx_log_grows_too_large_it_will_be_trimmed_while_queue_is_in_operation()
		{
			var txLogInfo = new FileInfo(Path.Combine(path, "transaction.log"));

			using (var queue = new PersistentQueue(path)
			{
				SuggestedMaxTransactionLogSize = 32 // single entry
			})
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 20; j++)
					{
						session.Enqueue(Guid.NewGuid().ToByteArray());
					}
					tx.Complete();
				}
				// there is no way optimize the file size here, 
				// so we should get expected size, even though it is bigger than
				// what we suggested as the max
				txLogInfo.Refresh();
				long txSizeWhenOpen = txLogInfo.Length;

				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					for (int j = 0; j < 20; j++)
					{
						session.Dequeue();
					}
					Assert.IsNull(session.Dequeue());

					tx.Complete();
				}
				txLogInfo.Refresh();
				Assert.Less(txLogInfo.Length, txSizeWhenOpen);
			}
		}
	}
}