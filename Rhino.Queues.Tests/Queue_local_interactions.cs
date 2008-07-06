using System;
using System.Threading;
using System.Transactions;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Data;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Queue_local_interactions
	{
		private Queue queue;

		private readonly QueueMessage msg = new QueueMessage
		{
			Body = new byte[] { 1, 2, 3, 4 }
		};

		private IIncomingMessageRepository stubbedIncomingMsgsRepos;
		private IOutgoingMessageRepository stubbedOutgoingMsgsRepos;

		[SetUp]
		public void Setup()
		{
			stubbedOutgoingMsgsRepos = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedIncomingMsgsRepos = MockRepository.GenerateStub<IIncomingMessageRepository>();
			queue = new Queue(new Uri("queue://my/test"), stubbedOutgoingMsgsRepos, stubbedIncomingMsgsRepos, QueueType.Input);
		}

		[Test]
		public void After_sending_item_to_queue_will_set_message_id()
		{
			var previousId = msg.Id;
			queue.Send(msg);
			Assert.IsFalse(previousId.Equals(msg.Id));
		}

		[Test]
		public void Sending_item_to_queue_will_place_it_on_database()
		{
			queue.AcceptMessages(msg);
			stubbedIncomingMsgsRepos.AssertWasCalled(x => x.Save(msg));
		}

		[Test]
		public void When_getting_message_and_no_messages_exists_will_return_null()
		{
			stubbedIncomingMsgsRepos.Stub(x => x.GetEarliestMessage()).Return(null);
			Assert.IsNull(queue.Recieve(TimeSpan.Zero));
		}

		[Test]
		public void When_getting_message_and_no_messages_exists_will_wait_until_timeout_expired_to_return_null()
		{
			stubbedIncomingMsgsRepos.Stub(x => x.GetEarliestMessage()).Return(null);
			DateTime start = DateTime.Now;
			Assert.IsNull(queue.Recieve(TimeSpan.FromMilliseconds(100)));
			Assert.GreaterThan(DateTime.Now, start.AddMilliseconds(99));
		}

		[Test]
		public void When_getting_message_and_no_messages_exists_will_wait_and_return_any_messages_that_arrive_meanwhile()
		{
			stubbedIncomingMsgsRepos.Stub(x => x.GetEarliestMessage()).Return(null);
			// second call will come after being waken
			stubbedIncomingMsgsRepos.Stub(x => x.GetEarliestMessage()).Return(msg);
			DateTime start = DateTime.Now;
			ThreadPool.QueueUserWorkItem(delegate
			{
				Thread.Sleep(50);
				queue.AcceptMessages(msg);
			});
			var recieved = queue.Recieve(TimeSpan.FromMilliseconds(100));
			Assert.IsNotNull(recieved);
			Assert.GreaterThan(start.AddMilliseconds(99), DateTime.Now);
		}

		[Test]
		public void When_running_in_transaction_will_not_notify_on_new_messages_arrived()
		{
			var hasGottenNewMessage = false;
			var startWaitingForNewMessages = new ManualResetEvent(false);
			var waitForNewMessagesCalled = new ManualResetEvent(false);
			ThreadPool.QueueUserWorkItem(delegate
			{
				startWaitingForNewMessages.WaitOne();
				hasGottenNewMessage = queue.WaitForNewMessageToArrive(TimeSpan.FromMilliseconds(10));
				waitForNewMessagesCalled.Set();
			});
			using (new TransactionScope())
			{
				startWaitingForNewMessages.Set();
				queue.AcceptMessages(msg);
			}
			waitForNewMessagesCalled.WaitOne();
			Assert.IsFalse(hasGottenNewMessage);
		}

		[Test]
		public void When_running_in_transaction_will_notify_on_new_messages_arrived_when_transaction_is_commited()
		{
			var hasGottenNewMessage = false;
			var startWaitingForNewMessages = new ManualResetEvent(false);
			var waitForNewMessagesCalled = new ManualResetEvent(false);
			ThreadPool.QueueUserWorkItem(delegate
			{
				startWaitingForNewMessages.WaitOne();
				hasGottenNewMessage = queue.WaitForNewMessageToArrive(TimeSpan.FromMilliseconds(50));
				waitForNewMessagesCalled.Set();
			});
			using (var tx = new TransactionScope())
			{
				startWaitingForNewMessages.Set();
				queue.AcceptMessages(msg);
				tx.Complete();
			}
			waitForNewMessagesCalled.WaitOne();
			Assert.IsTrue(hasGottenNewMessage);
		}
	}
}