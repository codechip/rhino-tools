using System;
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
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			{
				environment.DeleteQueue("my-queue");
				environment.CreateQueue("my-queue", 128);
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
							//tx2.Commit();
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
	}
}