using System;
using System.Threading;
using MbUnit.Framework;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class IntegrationTesting
	{
		private IQueueFactory factory1;
		private IQueueFactory factory2;

		[SetUp]
		public void Setup()
		{
			factory1 = new QueueConfiguration()
				.QueuesDirectory("factory1")
				.LocalUri("queue://localhost/factory1")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.SkipInitializingTheQueueFactory()
				.BuildQueueFactory();
			factory2 = new QueueConfiguration()
				.QueuesDirectory("factory2")
				.LocalUri("queue://localhost/factory2")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.SkipInitializingTheQueueFactory()
				.BuildQueueFactory();
		}

		[TearDown]
		public void Teardown()
		{
			factory1.Dispose();
			factory2.Dispose();
		}

		[Test]
		public void Can_send_from_one_queue_and_recieve_in_another()
		{
			factory1.Initialize();
			factory2.Initialize();

			IRemoteQueue remoteQueue = factory1.GetRemoteQueue(new Uri("queue://localhost/factory2"));
			remoteQueue.Send(new QueueMessage { Body = new byte[] { 1, 2, 3, 4 } });

			ILocalQueue localQueue = factory2.GetLocalQueue("factory2");
			QueueMessage recieve = localQueue.Recieve(TimeSpan.FromSeconds(60));
			CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, recieve.Body);
		}

		[Test]
		public void Can_send_from_one_queue_and_recieve_in_another_when_second_queue_is_turned_off_at_send_time()
		{
			factory1.Initialize();

			IRemoteQueue remoteQueue = factory1.GetRemoteQueue(new Uri("queue://localhost/factory2"));
			remoteQueue.Send(new QueueMessage { Body = new byte[] { 1, 2, 3, 4 } });

			factory2.Initialize();

			ILocalQueue localQueue = factory2.GetLocalQueue("factory2");
			QueueMessage recieve = localQueue.Recieve(TimeSpan.FromSeconds(60));
			CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, recieve.Body);
		}

		[Test]
		public void Can_send_from_one_queue_and_recieve_in_another_when_first_queue_is_turned_on_after_second_one_starts_recieving()
		{
			QueueMessage recieve = null;
			ManualResetEvent waitForMessageArrive = new ManualResetEvent(false);
			ThreadPool.QueueUserWorkItem(state =>
			{
				factory2.Initialize();

				ILocalQueue localQueue = factory2.GetLocalQueue("factory2");
				recieve = localQueue.Recieve(TimeSpan.FromSeconds(60));
				waitForMessageArrive.Set();
			});

			Thread.Sleep(100);
			factory1.Initialize();

			IRemoteQueue remoteQueue = factory1.GetRemoteQueue(new Uri("queue://localhost/factory2"));
			remoteQueue.Send(new QueueMessage { Body = new byte[] { 1, 2, 3, 4 } });
			waitForMessageArrive.WaitOne();

			CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, recieve.Body);
		}
	}
}