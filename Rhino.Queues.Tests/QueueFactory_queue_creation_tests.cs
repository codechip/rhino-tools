using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Workers;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueFactory_queue_creation_tests
	{
		QueueFactory queueFactory;
		private IOutgoingMessageRepository stubbedOutgoingMessageRepository;
		private IWorkerFactory stubbedWorkerFactory;
		private IQueueListener stubbedQueueListener;
		private IQueuePhysicalStorage stubbedQueryPhysicalStorage;

		[SetUp]
		public void Setup()
		{
			stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedWorkerFactory = MockRepository.GenerateStub<IWorkerFactory>();
			stubbedQueueListener = MockRepository.GenerateStub<IQueueListener>();
			stubbedQueryPhysicalStorage = MockRepository.GenerateStub<IQueuePhysicalStorage>();
			queueFactory = new QueueFactory(new Uri("queue://localhost/test"),
			                                Environment.CurrentDirectory,
											stubbedQueryPhysicalStorage,
			                                stubbedWorkerFactory,
			                                stubbedQueueListener,
			                                stubbedOutgoingMessageRepository);

			stubbedQueryPhysicalStorage.Stub(x => x.GetQueueNames())
				.Return(new[] {"foo"})
				.Repeat.Any();
			queueFactory.Initialize();
		}

		[Test]
		public void When_asking_for_existing_local_queue_will_return_queue_instance()
		{
			Assert.IsNotNull(queueFactory.GetLocalQueue("foo"));
		}

		[Test]
		public void When_asking_for_unknown_local_queue_will_return_null()
		{
			Assert.IsNull(queueFactory.GetLocalQueue("foo23"));
		}

		[Test]
		public void When_asking_for_existing_remote_queue_will_return_queue_instance()
		{
			Assert.IsNotNull(queueFactory.GetRemoteQueue("queue://foo/exists"));
		}

		[Test]
		public void When_asking_for_missing_remote_queue_will_return_queue_instance()
		{
			// we can't verify remote queues, so we will let it work
			Assert.IsNotNull(queueFactory.GetRemoteQueue("queue://foo/does-not-exists"));
		}


		[Test]
		public void When_creating_new_queue_will_call_queue_physical_storage_to_perform_the_work()
		{
			queueFactory.CreateQueue("foo2");
			stubbedQueryPhysicalStorage.AssertWasCalled(x=>x.CreateInputQueue("foo2"));
		}

		[Test]
		public void After_creating_queue_can_get_instance_of_queue()
		{
			Assert.IsNull(queueFactory.GetLocalQueue("foo23"));
			queueFactory.CreateQueue("foo23");
			Assert.IsNotNull(queueFactory.GetLocalQueue("foo23"));
		}
	}
}