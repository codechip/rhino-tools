using System;
using System.IO;
using MbUnit.Framework;
using Rhino.Queues.Impl;
using System.Linq;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueConfigurationTests
	{
		[SetUp]
		public void Setup()
		{
			if (Directory.Exists("Queues"))
				Directory.Delete("Queues", true);
			Directory.CreateDirectory("Queues");
		}

		[Test]
		public void Upon_start_will_create_queues_directory_if_does_not_exists()
		{
			Directory.Delete("Queues", true);
			Assert.IsFalse(Directory.Exists("Queues"));

			new QueueConfiguration()
				.QueuesDirectory("Queues")
				.BuildQueueFactory()
				.Dispose();

			Assert.IsTrue(Directory.Exists("Queues"));
		}

		[Test]
		public void Upon_start_will_create_outgoing_queue_if_does_not_exists()
		{
			bool queryExists = new BerkeleyDbPhysicalStorage("Queues").GetQueueNames()
				.Contains("outgoing-msgs");
			Assert.IsFalse(queryExists); 
			
			new QueueConfiguration()
				.QueuesDirectory("Queues")
				.BuildQueueFactory()
				.Dispose();

			queryExists = new BerkeleyDbPhysicalStorage("Queues").GetQueueNames()
				.Contains("outgoing-msgs");
			Assert.IsTrue(queryExists);
		}

		[Test]
		public void Can_get_factory_instance_from_configuration()
		{
			IQueueFactory factory = new QueueConfiguration()
				.BuildQueueFactory();
			Assert.IsNotNull(factory);
			factory.Dispose();
		}

		[Test]
		public void Setting_purge_messages_will_clear_messages_from_outgoing_queue_and_all_incoming_queues()
		{
			var outgoing = new OutgoingMessageRepository("outgoing-msgs", "Queues");
			var incoming1 = new IncomingMessageRepository("test", "Queues");
			var incoming2 = new IncomingMessageRepository("foo", "Queues");
			var storage = new BerkeleyDbPhysicalStorage("Queues");
			storage.CreateOutputQueue(outgoing.Name);
			storage.CreateInputQueue(incoming1.Name);
			storage.CreateInputQueue(incoming2.Name);

			outgoing.Save(new Uri("queue://foo/bar"), new QueueMessage());
			incoming1.Save(new QueueMessage());
			incoming2.Save(new QueueMessage());


			new QueueConfiguration()
				.QueuesDirectory("Queues")
				.PurgePendingMessages()
				.SkipInitializingTheQueueFactory()
				.BuildQueueFactory()
				.Dispose();

			Assert.AreEqual(0,
				outgoing.GetBatchOfMessagesToSend().DestinationBatches.Length);

			Assert.IsNull(incoming1.GetEarliestMessage());
			Assert.IsNull(incoming2.GetEarliestMessage());
		}

		[Test]
		public void If_local_uri_queue_does_not_exist_will_create_it()
		{
			Assert.IsFalse(
				new BerkeleyDbPhysicalStorage("Queues").GetQueueNames().Contains("my-fun")
				);

			new QueueConfiguration()
				.LocalUri("queue://localhost/my-fun")
				.QueuesDirectory("Queues")
				.PurgePendingMessages()
				.SkipInitializingTheQueueFactory()
				.BuildQueueFactory()
				.Dispose();

			Assert.IsTrue(
				new BerkeleyDbPhysicalStorage("Queues").GetQueueNames().Contains("my-fun")
				);
		}
	}
}