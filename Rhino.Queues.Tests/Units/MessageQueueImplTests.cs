using System;
using System.Collections.Generic;
using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;
using Rhino.Queues.Storage.InMemory;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class MessageQueueImplTests
	{
		private MessageQueueImpl queue;
		private InMemoryMessageStorage storage;

		[SetUp]
		public void SetUp()
		{
			storage = new InMemoryMessageStorage(new HashSet<string>
			{
                "test"
			});
			queue = new MessageQueueImpl(new Destination { Queue = "test" }, storage, storage, MockRepository.GenerateStub<IQueueFactoryImpl>());
		}

		[Test]
		public void When_getting_message_from_storage_will_get_first_message()
		{
			storage.Add("test", new TransportMessage { Message = 1 });
			storage.Add("test", new TransportMessage { Message = 2 });
			storage.Add("test", new TransportMessage { Message = 3 });
			Assert.AreEqual(1, queue.Recieve());
			Assert.AreEqual(2, queue.Recieve());
			Assert.AreEqual(3, queue.Recieve());
		}

		[Test]
		public void When_getting_message_from_storage_and_there_are_no_items_will_block()
		{
			DateTime start = DateTime.MaxValue;
			ThreadPool.QueueUserWorkItem(state =>
			{
				Thread.Sleep(100);
				start = DateTime.Now;
				storage.Add("test", new TransportMessage { Message = 1 });
			});
			Assert.AreEqual(1, queue.Recieve());
			Assert.LowerEqualThan(start, DateTime.Now);
		}
	}
}