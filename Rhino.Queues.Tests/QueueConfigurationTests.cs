using System;
using System.IO;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueConfigurationTests
	{
		[Test]
		public void Upon_start_will_create_queues_directory_if_does_not_exists()
		{
			string directory = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());

			Assert.IsFalse(Directory.Exists(directory));

			new QueueConfiguration()
				.QueuesDirectory(directory)
				.BuildQueueFactory()
				.Dispose();

			Assert.IsTrue(Directory.Exists(directory));

			Directory.Delete(directory, true);
		}

		[Test]
		public void Upon_start_will_create_outgoing_queue_if_does_not_exists()
		{
			string directory = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());

			new QueueConfiguration()
				.QueuesDirectory(directory)
				.BuildQueueFactory()
				.Dispose();

			Assert.IsTrue(File.Exists(Path.Combine(directory, "outgoing-msgs.queue")));

			Directory.Delete(directory, true);
		}

		[Test]
		public void Can_get_factory_instance_from_configuration()
		{
			IQueueFactory factory = new QueueConfiguration()
				.BuildQueueFactory();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void Setting_purge_messages_will_clear_messages_from_outgoing_queue_and_all_incoming_queues()
		{
			string directory = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());

			Directory.CreateDirectory(directory);

			var outgoing = new OutgoingMessageRepository("outgoing-msgs", directory);
			var incoming1 = new IncomingMessageRepository("test", directory);
			var incoming2 = new IncomingMessageRepository("foo", directory);
			incoming1.CreateQueueStorage();
			incoming2.CreateQueueStorage();
			outgoing.CreateQueueStorage();

			outgoing.Save(new Uri("queue://foo/bar"), new QueueMessage());
			incoming1.Save(new QueueMessage());
			incoming2.Save(new QueueMessage());


			new QueueConfiguration()
				.QueuesDirectory(directory)
				.PurgePendingMessages()
				.SkipInitializingTheQueueFactory()
				.BuildQueueFactory()
				.Dispose();

			Assert.AreEqual(0,
				outgoing.GetBatchOfMessagesToSend().DestinationBatches.Length);

			Assert.IsNull(incoming1.GetEarliestMessage());
			Assert.IsNull(incoming2.GetEarliestMessage());

			Directory.Delete(directory, true);
		}
	}
}