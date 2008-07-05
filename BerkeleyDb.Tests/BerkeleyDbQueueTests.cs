using System;
using System.IO;
using System.Threading;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class BerkeleyDbQueueTests
	{
		[SetUp]
		public void Setup()
		{
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			{
				environment.Delete("my-queue");
				environment.Delete("my-queue1");
				environment.Delete("my-queue2");
				environment.CreateQueue("my-queue", 128);
				environment.CreateQueue("my-queue1", 128);
				environment.CreateQueue("my-queue2", 128);
				tx.Commit();
			}
		}

		[Test]
		public void Can_get_queue_size()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var queue = environment.OpenQueue("my-queue"))
			{
				Assert.AreEqual(128, queue.MaxItemSize);
			}
		}

		[Test]
		public void Can_get_queue_name()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var queue = environment.OpenQueue("my-queue"))
			{
				Assert.AreEqual("my-queue", queue.Name);
			}
		}

		[Test]
		public void Can_append_and_consume_bytes_directly()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.AppendBytes(new byte[] { 1, 2, 3 });
				byte[] results = queue.ConsumeBytes();

				CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, results);

				tx.Commit();
			}
		}

		[Test]
		public void Can_append_item_to_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime());
				tx.Commit();
			}
		}

		[Test]
		public void Can_append_and_get_item_to_queue_in_same_transaction()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				var consumed = queue.Consume();
				Assert.IsNotNull(consumed);
				Assert.AreEqual(new DateTime(2000, 1, 1), consumed);
				tx.Commit();
			}
		}

		[Test]
		public void Can_append_and_get_item_to_queue_in_seperate_transactions()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var consumed = queue.Consume();
				Assert.IsNotNull(consumed);
				Assert.AreEqual(new DateTime(2000, 1, 1), consumed);
				tx.Commit();
			}
		}

		[Test]
		public void After_placing_in_queue_and_consuming_will_not_find_item_in_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				var consumed = queue.Consume();
				Assert.IsNotNull(consumed);
				consumed = queue.Consume();
				Assert.IsNull(consumed);
				tx.Commit();
			}
		}


		[Test]
		public void Rolling_back_consuming_a_message_will_place_it_back_in_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var consumed = queue.Consume();
				Assert.IsNotNull(consumed);
				Assert.AreEqual(new DateTime(2000, 1, 1), consumed);
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var consumed = queue.Consume();
				Assert.IsNotNull(consumed);
				Assert.AreEqual(new DateTime(2000, 1, 1), consumed);
				tx.Commit();
			}
		}


		[Test]
		public void When_transaction_rollbacked_will_not_find_it_in_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var consumed = queue.Consume();
				Assert.IsNull(consumed);
				tx.Commit();
			}
		}

		[Test]
		public void Can_consume_large_ammount_of_messages_in_separate_transactions()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				for (int i = 0; i < 1000; i++)
				{
					using (var tx = environment.BeginTransaction())
					using (var queue = environment.OpenQueue("my-queue"))
					{

						queue.Append(new DateTime(2000, 1, 1));
						tx.Commit();
					}
				}
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				for (int i = 0; i < 1000; i++)
				{
					using (var tx = environment.BeginTransaction())
					using (var queue = environment.OpenQueue("my-queue"))
					{

						Assert.AreEqual(new DateTime(2000, 1, 1),
							queue.ConsumeWithWait());
						tx.Commit();
					}
				}
			}
		}

		[Test]
		public void While_transaction_was_not_commited_cannot_consume_messages()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tx = environment.BeginTransaction())
				using (var queue = environment.OpenQueue("my-queue"))
				{

					queue.Append(new DateTime(2000, 1, 1));

					// now test that we cannot read it
					using (var environment2 = new BerkeleyDbEnvironment("test"))
					{
						using (var tx2 = environment2.BeginTransaction())
						using (var queue2 = environment2.OpenQueue("my-queue"))
						{
							Assert.IsNull(queue2.Consume());
							tx2.Commit();
						}
					}

					tx.Commit();
				}
			}
		}

		[Test]
		public void Can_consume_large_ammount_of_messages_in_single_transaction()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tx = environment.BeginTransaction())
				using (var queue = environment.OpenQueue("my-queue"))
				{
					for (int i = 0; i < 1000; i++)
					{
						queue.Append(new DateTime(2000, 1, 1));
					}
					tx.Commit();
				}
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tx = environment.BeginTransaction())
				using (var queue = environment.OpenQueue("my-queue"))
				{
					for (int i = 0; i < 1000; i++)
					{

						Assert.AreEqual(new DateTime(2000, 1, 1),
							queue.ConsumeWithWait());
					}
					tx.Commit();
				}
			}
		}

		[Test]
		public void Can_consume_large_ammount_of_messages_in_single_transaction_multi_threaded()
		{
			DateTime finishedProducing = DateTime.MinValue;
			DateTime startedReading = DateTime.MinValue;

			var producer = new Thread(() =>
			{
				using (var environment = new BerkeleyDbEnvironment("test"))
				{
					using (var tx = environment.BeginTransaction())
					using (var queue = environment.OpenQueue("my-queue"))
					{
						for (int i = 0; i < 1000; i++)
						{
							queue.Append(new DateTime(2000, 1, 1));
						}
						finishedProducing = DateTime.Now;
						tx.Commit();
					}
				}

			});
			var consumer = new Thread(() =>
			{
				using (var environment = new BerkeleyDbEnvironment("test"))
				{
					using (var tx = environment.BeginTransaction())
					using (var queue = environment.OpenQueue("my-queue"))
					{
						bool firstRead = true;
						for (int i = 0; i < 1000; i++)
						{
							var wait = queue.ConsumeWithWait();
							if (firstRead)
							{
								startedReading = DateTime.Now;
								firstRead = false;
							}
							Assert.AreEqual(new DateTime(2000, 1, 1),
											wait);
						}
						tx.Commit();
					}
				}
			});

			producer.Start();
			consumer.Start();

			producer.Join();
			consumer.Join();

			Assert.GreaterThan(startedReading, finishedProducing);
		}

		[Test]
		public void Can_create_transaction_that_spans_across_multiple_queues()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				queue1.Append(new DateTime(2001, 1, 1));
				queue2.Append(new DateTime(2002, 1, 1));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				var consumed1 = queue1.Consume();
				var consumed2 = queue2.Consume();
				Assert.AreEqual(consumed1, new DateTime(2001, 1, 1));
				Assert.AreEqual(consumed2, new DateTime(2002, 1, 1));
				tx.Commit();
			}
		}

		[Test]
		public void Multi_queue_transactions_when_rollback_will_remove_from_all_queues()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				queue1.Append(new DateTime(2001, 1, 1));
				queue2.Append(new DateTime(2002, 1, 1));
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				var consumed1 = queue1.Consume();
				var consumed2 = queue2.Consume();
				Assert.IsNull(consumed1);
				Assert.IsNull(consumed2);
				tx.Commit();
			}
		}

		[Test]
		public void When_consuming_from_multiple_queues_under_transaction_and_rolling_back_items_are_in_the_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				queue1.Append(new DateTime(2001, 1, 1));
				queue2.Append(new DateTime(2002, 1, 1));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				var consumed1 = queue1.Consume();
				var consumed2 = queue2.Consume();
				Assert.AreEqual(consumed1, new DateTime(2001, 1, 1));
				Assert.AreEqual(consumed2, new DateTime(2002, 1, 1));
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue1 = environment.OpenQueue("my-queue1"))
			using (var queue2 = environment.OpenQueue("my-queue2"))
			{
				var consumed1 = queue1.Consume();
				var consumed2 = queue2.Consume();
				Assert.AreEqual(consumed1, new DateTime(2001, 1, 1));
				Assert.AreEqual(consumed2, new DateTime(2002, 1, 1));
				tx.Commit();
			}
		}

		[Test]
		public void Can_truncate_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append("1234");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Truncate();
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				object value = queue.Consume();
				Assert.IsNull(value);
				tx.Commit();
			}
		}
	}
}