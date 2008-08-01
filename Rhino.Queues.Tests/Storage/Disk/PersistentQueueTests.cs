namespace Rhino.Queues.Tests.Storage.Disk
{
	using System;
	using System.IO;
	using System.Transactions;
	using MbUnit.Framework;
	using Rhino.Queues.Storage.Disk;

	[TestFixture]
	public class PersistentQueueTests : PersistentQueueTestsBase
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Another instance of the queue is already in action")]
		public void Only_single_instance_of_queue_can_exists_at_any_one_time()
		{
			using (new PersistentQueue(path))
			{
				new PersistentQueue(path);
			}
		}

		[Test]
		public void Can_create_new_queue()
		{
			new PersistentQueue(path).Dispose();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Unexpected data in transaction log. Expected to get transaction separator but got truncated data")]
		public void Corrupt_index_file_should_throw()
		{
			File.WriteAllBytes(Path.Combine(path, "transaction.log"), new byte[] { 12, 23, 42, 12 });
			new PersistentQueue(path);
		}

		[Test]
		public void Dequeqing_from_empty_queue_will_return_null()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				Assert.IsNull(session.Dequeue());
			}
		}


		[Test]
		public void Can_enqueue_data_in_queue()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}
		}

		[Test]
		public void Can_dequeue_data_from_queue()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void Can_enqueue_and_dequeue_data_after_restarting_queue()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void After_dequeue_from_queue_item_no_longer_on_queue()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				Assert.IsNull(session.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void After_dequeue_from_queue_item_no_longer_on_queue_with_queues_restarts()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				Assert.IsNull(session.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void Not_flushing_the_session_will_revert_dequequed_items()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				//Explicitly ommitted: tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void Not_flushing_the_session_will_revert_dequequed_items_two_sessions_same_queue()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session2 = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				using (var session1 = queue.OpenSession())
				using (var tx2 = new TransactionScope(TransactionScopeOption.RequiresNew))
				{
					CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session1.Dequeue());
					//Explicitly ommitted: tx2.Complete();
				}
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session2.Dequeue());
				tx.Complete();
			}
		}

		[Test]
		public void Two_sessions_off_the_same_queue_cannot_get_same_item()
		{
			using (var queue = new PersistentQueue(path))
			using (var session = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				session.Enqueue(new byte[] { 1, 2, 3, 4 });
				tx.Complete();
			}

			using (var queue = new PersistentQueue(path))
			using (var session2 = queue.OpenSession())
			using (var session1 = queue.OpenSession())
			using (var tx = new TransactionScope())
			{
				CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, session1.Dequeue());
				Assert.IsNull(session2.Dequeue());
			}
		}

		[Test]
		public void Reversing_dequeue_will_send_to_end_of_line()
		{
			using (var queue = new PersistentQueue(path))
			{
				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					session.Enqueue(BitConverter.GetBytes(1));
					session.Enqueue(BitConverter.GetBytes(2));
					session.Enqueue(BitConverter.GetBytes(3));
					tx.Complete();
				}

				using (var session = queue.OpenSession())
				using (var tx = new TransactionScope())
				{
					Action reverse;
					Assert.AreEqual(1, BitConverter.ToInt32(session.ReversibleDequeue(out reverse), 0));
					reverse();
					Assert.AreEqual(2, BitConverter.ToInt32(session.Dequeue(), 0));
					Assert.AreEqual(3, BitConverter.ToInt32(session.Dequeue(), 0));
					Assert.AreEqual(1, BitConverter.ToInt32(session.Dequeue(), 0));
					
					tx.Complete();
				}
			}


		}
	}
}