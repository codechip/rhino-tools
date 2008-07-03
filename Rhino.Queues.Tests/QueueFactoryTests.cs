using System;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueFactoryTests
	{

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "The outgoing queue 'outgoing-queue' is reserved for Rhino Queues use")]
		public void Cannot_get_remote_output_queue_directly()
		{
			var stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedOutgoingMessageRepository.Stub(x => x.Name)
				.Return("outgoing-queue")
				.Repeat.Any();

			var queueFactory = CreateQueue(stubbedOutgoingMessageRepository, "queue://localhost/test");
			queueFactory.GetRemoteQueue(new Uri("queue://localhost/outgoing-queue"));
		}

		private QueueFactory CreateQueue(IOutgoingMessageRepository stubbedOutgoingMessageRepository, string name)
		{
			return new QueueFactory(new Uri(name), Environment.CurrentDirectory, null, null, null, stubbedOutgoingMessageRepository);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException), "The outgoing queue 'outgoing-queue' is reserved for Rhino Queues use")]
		public void Cannot_get_local_output_queue_directly()
		{
			var stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedOutgoingMessageRepository.Stub(x => x.Name)
				.Return("outgoing-queue")
				.Repeat.Any();

			var queueFactory = CreateQueue(stubbedOutgoingMessageRepository, "queue://localhost/test");
			queueFactory.GetLocalQueue("outgoing-queue");
		} 


		[Test]
		public void IsLocal_when_local()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://localhost/test"));
			Assert.IsTrue(local);
		}

		[Test]
		public void IsLocal_when_local_using_local_IP()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://127.0.0.1/test"));
			Assert.IsTrue(local);
		}

		[Test]
		public void IsLocal_when_local_using_local_IP_with_port()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://127.0.0.1:123/test"));
			Assert.IsFalse(local);
		}

		[Test]
		public void IsLocal_when_local_using_local_IP_with_different_ports()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://127.0.0.1:80/test"));
			Assert.IsTrue(local);
		}

		[Test]
		public void IsLocal_when_local_using_local_IP_explicitly_specifying_default_port()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://127.0.0.1:80/test"));
			Assert.IsTrue(local);
		}

		[Test]
		public void IsLocal_when_remote()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://ayende/blah"));
			Assert.IsFalse(local);
		}

		[Test]
		public void IsLocal_when_remote_using_IP()
		{
			var queueFactory = CreateQueue(null, "queue://localhost/test");
			bool local = queueFactory.IsLocal(new Uri("queue://9.3.2.4/blah"));
			Assert.IsFalse(local);
		}
	}
}