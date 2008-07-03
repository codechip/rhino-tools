using System;
using System.IO;
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
			if (File.Exists("test.queue"))
				File.Delete("test.queue");
			outgoingMessageRepository = new OutgoingMessageRepository("test");
			outgoingMessageRepository.CreateQueueStorage();
		}

		[Test]
		public void Can_save_message_to_outgoing_queue()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test"), new QueueMessage());
			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "select Destination from Messages";
				using (var reader = cmd.ExecuteReader())
				{
					reader.Read();
					Assert.AreEqual("queue://test/test", reader[0]);
				}
			}
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
			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "select SendAt from Messages";
				using (var reader = cmd.ExecuteReader())
				{
					reader.Read();
					Assert.AreEqual(new DateTime(2000,1,1), reader[0]);
				}
			}
		}

		[Test]
		public void When_purging_messages_will_remove_all_active_and_failed_messages()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			MessageBatch send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.MoveUnderliverableMessagesToDeadLetterQueue(send.Id, new Uri("queue://test/test2"), 0, new Exception());

			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "select count(*) from Messages";
				Assert.AreEqual(1, cmd.ExecuteScalar());
				cmd.CommandText = "select count(*) from FailedMessages";
				Assert.AreEqual(1, cmd.ExecuteScalar());
			}

			outgoingMessageRepository.PurgeAllMessages();

			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "select count(*) from Messages";
				Assert.AreEqual(0, cmd.ExecuteScalar());
				cmd.CommandText = "select count(*) from FailedMessages";
				Assert.AreEqual(0, cmd.ExecuteScalar());
			}

		}

		[Test]
		public void When_moving_to_dead_letter_queue_will_use_batch_id_and_destination_as_filters()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			MessageBatch send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.MoveUnderliverableMessagesToDeadLetterQueue(send.Id, new Uri("queue://test/test2"), 0, new Exception());

			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "select count(*) from Messages";
				Assert.AreEqual(1, cmd.ExecuteScalar());
				cmd.CommandText = "select count(*) from FailedMessages";
				Assert.AreEqual(1, cmd.ExecuteScalar());
			}
		}
	}
}