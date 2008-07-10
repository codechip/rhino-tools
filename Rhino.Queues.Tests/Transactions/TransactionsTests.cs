using System.Transactions;
using MbUnit.Framework;
using Rhino.Queues.Cfg;
using Rhino.Queues.Impl;
using System.Linq;

namespace Rhino.Queues.Tests.Transactions
{
	[TestFixture]
	public class TransactionsTests
	{
		private QueueFactoryImpl serverFactory;

		[SetUp]
		public void SetUp()
		{
			serverFactory = new Configuration("server")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueues("foo", "bar")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory() as QueueFactoryImpl;
		}

		[TearDown]
		public void TearDown()
		{
			serverFactory.Dispose();
		}

		[Test]
		public void When_running_in_transaction_will_not_add_messages_to_storage_when_transaction_is_active()
		{
			serverFactory.MarkAsStarted();
			using (new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo@client"))
			{
				queue.Send(1);
				Assert.IsNull(
					serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault()
					);
			}
		}

		[Test]
		public void When_running_in_transaction_and_transaction_was_committed_will_add_message_to_storage()
		{
			serverFactory.MarkAsStarted();
			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo@client"))
			{
				queue.Send(1);
				tx.Complete();
			}
			Assert.IsNotNull(
					serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault()
					);
		}

		[Test]
		public void When_running_in_transaction_and_removing_from_queue_messages_are_not_longer_on_queue()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				queue.Send(1);
				tx.Complete();
			}

			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				Assert.AreEqual(1, queue.Recieve().Value);
				Assert.IsNull(
					serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault()
					);
				tx.Complete();
			}

		}

		[Test]
		public void When_transaction_is_rolled_back_message_goes_back_to_queue()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				queue.Send(1);
				tx.Complete();
			}

			using (new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				Assert.AreEqual(1, queue.Recieve().Value);
				Transaction.Current.Rollback();
			}

			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				Assert.AreEqual(1, queue.Recieve().Value);
				tx.Complete();
			}
		}

		[Test]
		public void When_transaction_is_not_committed_message_goes_back_to_queue()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				queue.Send(1);
				tx.Complete();
			}

			using (new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				Assert.AreEqual(1, queue.Recieve().Value);
			}

			using (var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("foo"))
			{
				Assert.AreEqual(1, queue.Recieve().Value);
				tx.Complete();
			}
		}

		[Test]
		public void When_running_in_transaction_will_not_add_messages_to_storage_when_transaction_is_active_with_multiple_queues()
		{
			serverFactory.MarkAsStarted();
			using (new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo@client"))
			using (var barQueue = serverFactory.OpenQueue("bar@client"))
			{
				barQueue.Send(1);
				fooQueue.Send(2);

				Assert.IsNull(
					serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault()
					);
			}
		}

		[Test]
		public void When_running_in_transaction_and_transaction_was_committed_will_add_message_to_storage_with_multiple_queues()
		{
			serverFactory.MarkAsStarted();
			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo@client"))
			using (var barQueue = serverFactory.OpenQueue("bar@client"))
			{
				fooQueue.Send(1);
				barQueue.Send(2);
				tx.Complete();
			}
			Assert.AreEqual(2,
					serverFactory.OutgoingStorage
						.PullMessagesFor("http://localhost:9999/client/").Count()
					);
		}
		
		[Test]
		public void When_running_in_transaction_and_removing_from_queue_messages_are_not_longer_on_queue_with_multiple_queues()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				fooQueue.Send(1);
				barQueue.Send(2);
				tx.Complete();
			}

			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				Assert.AreEqual(1, fooQueue.Recieve().Value);
				Assert.AreEqual(2, barQueue.Recieve().Value);
				Assert.IsNull(
					serverFactory.OutgoingStorage.PullMessagesFor("http://localhost:9999/client/").FirstOrDefault()
					);
				tx.Complete();
			}
		}

		[Test]
		public void When_transaction_is_rolled_back_message_goes_back_to_queue_with_multiple_queues()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				fooQueue.Send(1);
				barQueue.Send(2);
				tx.Complete();
			}

			using (new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				Assert.AreEqual(1, fooQueue.Recieve().Value);
				Assert.AreEqual(2, barQueue.Recieve().Value);
				Transaction.Current.Rollback();
			}

			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				Assert.AreEqual(1, fooQueue.Recieve().Value);
				Assert.AreEqual(2, barQueue.Recieve().Value);
				tx.Complete();
			}
		}

		[Test]
		public void When_transaction_is_not_committed_message_goes_back_to_queue_with_multiple_queues()
		{
			serverFactory.Start();
			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				fooQueue.Send(1);
				barQueue.Send(2);
				tx.Complete();
			}

			using (new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				Assert.AreEqual(1, fooQueue.Recieve().Value);
				Assert.AreEqual(2, barQueue.Recieve().Value);
			}

			using (var tx = new TransactionScope())
			using (var fooQueue = serverFactory.OpenQueue("foo"))
			using (var barQueue = serverFactory.OpenQueue("bar"))
			{
				Assert.AreEqual(1, fooQueue.Recieve().Value);
				Assert.AreEqual(2, barQueue.Recieve().Value);
				tx.Complete();
			}
		}
	}
}