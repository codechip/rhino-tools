using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;
using Rhino.Queues.Workers;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class WorkerFactoryTests
	{
		[Test]
		public void After_starting_workers_will_have_worker_threads_running()
		{
			var stubbedQueueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Repeat.Any();

			var factory = new WorkerFactory(5, "test");
			factory.StartWorkers(stubbedQueueFactory, stubbedOutgoingMessageRepository);
			Assert.AreEqual(5, factory.Threads.Count);
			foreach (var thread in factory.Threads)
			{
				Assert.AreNotEqual("", thread.Name);
				Assert.IsTrue(thread.IsAlive);
				Assert.IsTrue(thread.IsBackground);
			}
		}

		[Test]
		public void Can_stop_and_join_all_workers()
		{
			var stubbedQueueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var stubbedOutgoingMessageRepository = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedOutgoingMessageRepository
				.Stub(x => x.GetBatchOfMessagesToSend())
				.Return(new MessageBatch())
				.Repeat.Any();

			var factory = new WorkerFactory(5, "test");
			factory.StartWorkers(stubbedQueueFactory, stubbedOutgoingMessageRepository);
			factory.StopWorkers();
			factory.WaitForAllWorkersToStop();

			foreach (var thread in factory.Threads)
			{
				Assert.IsFalse(thread.IsAlive);
			}
		}

		[Test]
		public void Asking_worker_factory_to_notify_will_notify_all_threads()
		{
			var factory = new WorkerFactory(5 ,"test");
			for (int i = 0; i < 5; i++)
			{
				factory.Workers.Add(MockRepository.GenerateStub<IQueueWorker>());
			}
			factory.NotifyAllWorkersThatNewMessageWasStored();
			foreach (var worker in factory.Workers)
			{
				worker.AssertWasCalled(x => x.NewMessageStored());
			}
		}
	}
}