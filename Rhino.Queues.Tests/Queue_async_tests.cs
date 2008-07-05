using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Queue_async_tests
	{
		[Test]
		public void When_registered_to_message_arrived_event_will_get_called_on_message_arrival()
		{
			var stubbedIncomingMessageRepository = MockRepository.GenerateStub<IIncomingMessageRepository>();
			var queue = new Queue(new Uri("queue://localhost/testQueue"),
			                      MockRepository.GenerateStub<IOutgoingMessageRepository>(),
			                      stubbedIncomingMessageRepository);

			stubbedIncomingMessageRepository
				.Stub(x => x.GetEarliestMessage())
				.Return(new QueueMessage());

			var wasCalled = false;
			var e = new ManualResetEvent(false);
			queue.MessageArrived+=(obj =>
			{
				wasCalled = true;
				e.Set();
			});
			queue.AcceptMessages(new QueueMessage());
			e.WaitOne();
			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void When_running_in_transaction_message_arrived_will_be_raised_after_transaction_commit()
		{
			var stubbedIncomingMessageRepository = MockRepository.GenerateStub<IIncomingMessageRepository>();
			var queue = new Queue(new Uri("queue://localhost/testQueue"),
								  MockRepository.GenerateStub<IOutgoingMessageRepository>(),
								  stubbedIncomingMessageRepository);

			stubbedIncomingMessageRepository
				.Stub(x => x.GetEarliestMessage())
				.Return(new QueueMessage());

			var wasCalled = false;
			var e = new ManualResetEvent(false);
			queue.MessageArrived += (obj =>
			{
				wasCalled = true;
				e.Set();
			});
			using (var tx = new TransactionScope())
			{
				queue.AcceptMessages(new QueueMessage());
				e.WaitOne(TimeSpan.FromMilliseconds(100),false);
				Assert.IsFalse(wasCalled); 
				tx.Complete();
			}
			e.WaitOne();
			Assert.IsTrue(wasCalled);
		}

		[Test] // competing customers
		public void When_registered_to_message_arrived_event_but_was_recieved_by_other_means_first_will_not_be_called()
		{
			var stubbedIncomingMessageRepository = MockRepository.GenerateStub<IIncomingMessageRepository>();
			var queue = new Queue(new Uri("queue://localhost/testQueue"),
								  MockRepository.GenerateStub<IOutgoingMessageRepository>(),
								  stubbedIncomingMessageRepository);

			// do not return a value, marking that someone else
			// has already dealt with this message, so we don't have to
			stubbedIncomingMessageRepository
				.Stub(x => x.GetEarliestMessage())
				.Return(null);

			var wasCalled = false;
			var e = new ManualResetEvent(false);
			queue.MessageArrived += (obj =>
			{
				wasCalled = true;
				e.Set();
			});
			queue.AcceptMessages(new QueueMessage());
			e.WaitOne(TimeSpan.FromMilliseconds(100),false);
			Assert.IsFalse(wasCalled);
		}


		[Test]
		public void Will_raise_message_arrived_event_for_each_message_in_batch_and_across_batches()
		{
			var stubbedIncomingMessageRepository = new FakeIncomingMessageRepository();

			var queue = new Queue(new Uri("queue://localhost/testQueue"),
								  MockRepository.GenerateStub<IOutgoingMessageRepository>(),
								  stubbedIncomingMessageRepository);

			var callCount = 0;
			var e = new ManualResetEvent(false);
			queue.MessageArrived += (obj =>
			{
				if (Interlocked.Increment(ref callCount) >= 100)
					e.Set();
			});
			for (int i = 0; i < 50; i++)
			{
				queue.AcceptMessages(
					new QueueMessage(), 
					new QueueMessage()
					);
			}
			e.WaitOne();
			Assert.AreEqual(100, callCount);
		}
	}

	internal class FakeIncomingMessageRepository : IIncomingMessageRepository
	{
		private Queue<QueueMessage> q = new Queue<QueueMessage>();

		public QueueMessage GetEarliestMessage()
		{
			lock(q)
			{
				if(q.Count==0)
					return null;
				return q.Dequeue();
			}
		}

		public void Save(params QueueMessage[] msgs)
		{
			lock (q)
			{
				foreach (var msg in msgs)
				{
					q.Enqueue(msg);
				}
			}
		}

		public void PurgeAllMessages()
		{
			lock(q)
			{
				q.Clear();
			}
		}
	}
}