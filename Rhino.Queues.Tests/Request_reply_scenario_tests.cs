using System;
using System.IO;
using System.Text;
using System.Threading;
using MbUnit.Framework;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Request_reply_scenario_tests
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
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();

		}


		[TearDown]
		public void Teardown()
		{
			factory1.Dispose();
			factory2.Dispose();
		}

		[Test]
		public void Can_send_message_and_reply_to_it()
		{
			IRemoteQueue remoteQueue1 = factory1.GetRemoteQueue("queue://localhost/factory2");
			ILocalQueue localQueue1 = factory1.GetLocalQueue("factory1");

			IRemoteQueue remoteQueue2 = factory2.GetRemoteQueue(new Uri("queue://localhost/factory1"));
			ILocalQueue localQueue2 = factory2.GetLocalQueue("factory2");

			string firstMsg = "hello world";
			string secondMsg = "hello you too";

			var firstArrived = new ManualResetEvent(false);
			var secondArrived = new ManualResetEvent(false);
			localQueue2.MessageArrived+=delegate(QueueMessage message)
			{
				string msgTxt = Encoding.UTF8.GetString(message.Body);
				Assert.AreEqual(firstMsg,msgTxt);
				firstArrived.Set();
				remoteQueue2.Send(new QueueMessage{Body = Encoding.UTF8.GetBytes(secondMsg)});
			};

			localQueue1.MessageArrived+=delegate(QueueMessage message)
			{
				string msgTxt = Encoding.UTF8.GetString(message.Body);
				Assert.AreEqual(secondMsg,msgTxt);
				secondArrived.Set();
			};
			remoteQueue1.Send(new QueueMessage { Body = Encoding.UTF8.GetBytes(firstMsg) });
			WaitHandle.WaitAll(new WaitHandle[] {firstArrived, secondArrived,});
		}
	}
}