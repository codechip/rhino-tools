using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MbUnit.Framework;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Many_messages_secnario_tests
	{
		private IQueueFactory factory1;
		private IQueueFactory factory2;

		[SetUp]
		public void Setup()
		{
			SystemTime.Now = () => DateTime.Now;

			if (Directory.Exists("factory1"))
				Directory.Delete("factory1", true);
			if (Directory.Exists("factory2"))
				Directory.Delete("factory2", true);

			factory1 = new QueueConfiguration()
				.QueuesDirectory("factory1")
				.LocalUri("queue://localhost/factory1")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();
			factory2 = new QueueConfiguration()
				.QueuesDirectory("factory2")
				.LocalUri("queue://localhost/factory2")
				.PurgePendingMessages()
				.WorkerThreads(1)
				.BuildQueueFactory();
		}


		[TearDown]
		public void Teardown()
		{
			factory1.Dispose();
			factory2.Dispose();
		}


		[Test]
		public void Can_send_hundreds_of_messages_and_they_all_get_through()
		{
			IRemoteQueue remoteQueue1 = factory1.GetRemoteQueue(new Uri("queue://localhost/factory2"));
			ILocalQueue localQueue2 = factory2.GetLocalQueue("factory2");

			var values = new List<int>();
			var doneGettingMessages = new ManualResetEvent(false);
			localQueue2.MessageArrived += delegate(QueueMessage message)
			{
				lock (values)
				{
					Console.WriteLine("Values at : " + values.Count);
					values.Add(BitConverter.ToInt32(message.Body, 0));
					if (values.Count == 250)
						doneGettingMessages.Set();
				}
			};
			for (int i = 0; i < 250; i++)
			{
				remoteQueue1.Send(new QueueMessage { Body = BitConverter.GetBytes(i) });
			}
			doneGettingMessages.WaitOne();
			int prev = -1;
			Assert.AreEqual(250, values.Count);
			values.Sort();// we do not give any ordering guarantees
			foreach (var val in values)
			{
				Assert.AreEqual(prev + 1, val);
				prev++;
			}
		}
	}
}