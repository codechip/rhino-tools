using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;
using Rhino.Queues.Workers;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueFactory_initialization_tests
	{
		QueueFactory queueFactory;
		private IOutgoingMessageRepository stubbedOutgoingMessageRepository;
		private IWorkerFactory stubbedWorkerFactory;
		private IQueueListener stubbedQueueListener;
		private IQueuePhysicalStorage stubbedQueuePhysicalStorage;

		[SetUp]
		public void Setup()
		{
			stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedWorkerFactory = MockRepository.GenerateStub<IWorkerFactory>();
			stubbedQueueListener = MockRepository.GenerateStub<IQueueListener>();
			stubbedQueuePhysicalStorage = MockRepository.GenerateStub<IQueuePhysicalStorage>();
			queueFactory = new QueueFactory(new Uri("queue://localhost/test"),
				Environment.CurrentDirectory,
				stubbedQueuePhysicalStorage,
				stubbedWorkerFactory,
				stubbedQueueListener,
				stubbedOutgoingMessageRepository);

			stubbedQueuePhysicalStorage.Stub(x => x.GetQueueNames())
				.Return(new[] {"a", "b", "c"})
				.Repeat.Any();
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),
			"The queue factory was already initialized")]
		public void Cannot_call_init_twice()
		{
			queueFactory.Initialize();
			queueFactory.Initialize();
		}

		[Test]
		public void Upon_startup_will_start_listener()
		{
			queueFactory.Initialize();
			stubbedQueueListener.AssertWasCalled(x=>x.Start(queueFactory));
		}

		[Test]
		public void Upon_startup_will_create_instances_of_all_existing_queues()
		{
			queueFactory.Initialize();
			IQueueImpl[] queues = queueFactory.GetAllLocalQueues();
			Assert.AreEqual(3, queues.Length);
			Array.Sort(queues, (x,y) => x.Url.AbsolutePath.CompareTo(y.Url.AbsolutePath));
			Assert.AreEqual("a", queues[0].Url.LocalPath.Substring(1));
			Assert.AreEqual("b", queues[1].Url.LocalPath.Substring(1));
			Assert.AreEqual("c", queues[2].Url.LocalPath.Substring(1));
		}

		[Test]
		public void When_raising_new_message_stored_will_invoke_new_message_stored_on_worker()
		{
			queueFactory.Initialize();
			stubbedOutgoingMessageRepository.Raise(x => x.NewMessageStored += null);
			stubbedWorkerFactory.AssertWasCalled(x=>x.NotifyAllWorkersThatNewMessageWasStored());
		}


		[Test]
		public void Upon_startup_will_reset_all_batches_in_outgoing_queue()
		{
			queueFactory.Initialize();

			stubbedOutgoingMessageRepository.AssertWasCalled(x => x.ResetAllBatches());
		}

		[Test]
		public void Upon_startup_will_start_all_workers()
		{
			queueFactory.Initialize();

			stubbedWorkerFactory
				.AssertWasCalled(x => x.StartWorkers(queueFactory, stubbedOutgoingMessageRepository));
		}

		[Test]
		public void Can_use_queue_factory_to_get_local_queue()
		{
			queueFactory.Initialize();
			var queueUrl = new Uri("queue://localhost/a");
			ILocalQueue queue = queueFactory.GetLocalQueue("a");
			Assert.IsNotNull(queue);
			Assert.AreEqual(queueUrl, ((Queue)queue).Url);
		}

		[Test]
		public void When_asking_for_remote_queue_on_local_factory_will_give_same_instance_as_used_by_name()
		{
			queueFactory.Initialize();
			ILocalQueue queue = queueFactory.GetLocalQueue("test");
			Assert.AreSame(queue,
				queueFactory.GetRemoteQueue(new Uri("queue://localhost/test")));
		}

		[Test]
		public void Will_return_same_instance_of_remote_queue_when_requested_twice()
		{
			queueFactory.Initialize();
			var queueUrl = new Uri("queue://localhost/test");
			IRemoteQueue queue1 = queueFactory.GetRemoteQueue(queueUrl);
			IRemoteQueue queue2 = queueFactory.GetRemoteQueue(queueUrl);
			Assert.AreSame(queue1, queue2);
		}

		[Test]
		public void Will_return_same_instance_of_local_queue_when_requested_twice()
		{
			queueFactory.Initialize();
			ILocalQueue queue1 = queueFactory.GetLocalQueue("test");
			ILocalQueue queue2 = queueFactory.GetLocalQueue("test");
			Assert.AreSame(queue1, queue2);
		}
	}
}