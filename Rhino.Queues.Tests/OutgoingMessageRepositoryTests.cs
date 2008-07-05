using System;
using System.IO;
using BerkeleyDb;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class OutgoingMessageRepositoryTests
	{
		private OutgoingMessageRepository outgoingMessageRepository;

		[SetUp]
		public void Setup()
		{
			SystemTime.Now = () => new DateTime(2000, 1, 1);
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");

			outgoingMessageRepository = new OutgoingMessageRepository("test", "test");
			new BerkeleyDbPhysicalStorage("test").CreateOutputQueue("test");
		}

		[Test]
		public void Can_save_message_to_outgoing_queue()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test"), new QueueMessage());
			Assert.AreEqual(1,
				outgoingMessageRepository.GetBatchOfMessagesToSend().DestinationBatches.Length);
		}

		[Test]
		public void When_saving_message_to_outgoing_queue_will_raise_event()
		{
			bool wasCalled = false;
			outgoingMessageRepository.NewMessageStored += () => wasCalled = true;
			outgoingMessageRepository.Save(new Uri("queue://test/test"), new QueueMessage());
			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void When_saving_will_set_send_at_to_current_time()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test"), new QueueMessage());
			using (var env = new BerkeleyDbEnvironment("test"))
			using (var tree = env.OpenTree("test.tree"))
			using (var queue = env.OpenQueue("test.queue"))
			{
				var msg = (QueueTransportMessage)tree.Get(queue.Consume());
				Assert.AreEqual(new DateTime(2000, 1, 1),
					msg.SendAt);
			}
		}
	}
}