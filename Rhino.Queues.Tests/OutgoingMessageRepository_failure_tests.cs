using System;
using System.IO;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class OutgoingMessageRepository_failure_tests
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
		public void When_update_failure_count_will_raise_failure_count_for_all_items_in_batch_and_destination()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			AssertFailureCount(0);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));

			AssertFailureCount(1);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));

			AssertFailureCount(2);
		}

		[Test]
		public void When_update_failure_count_will_update_send_at_time()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			AssertSendAtEq(SystemTime.Now());

			//first time it doesn't update, so we will pick it up on the next update
			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			AssertSendAtEq(SystemTime.Now().AddSeconds(0));

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			AssertSendAtEq(SystemTime.Now().AddSeconds(1));

		}

		[Test]
		public void When_update_failure_count_will_update_send_at_time_in_increasing_intervals()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			var expectedSendAt = SystemTime.Now();
			AssertSendAtEq(expectedSendAt);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			expectedSendAt = expectedSendAt.AddSeconds(0);
			AssertSendAtEq(expectedSendAt);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			expectedSendAt = expectedSendAt.AddSeconds(1);
			AssertSendAtEq(expectedSendAt);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			expectedSendAt = expectedSendAt.AddSeconds(3);
			AssertSendAtEq(expectedSendAt);

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));
			expectedSendAt = expectedSendAt.AddSeconds(6);
			AssertSendAtEq(expectedSendAt);
		}


		private void AssertFailureCount(int failureCount)
		{
			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT FailureCount FROM Messages";
				using (var reader = cmd.ExecuteReader())
				{
					int count = 0;
					while (reader.Read())
					{
						count += 1;
						Assert.AreEqual(failureCount, reader.GetInt32(0));
					}
					Assert.AreEqual(2, count);
				}
			}
		}

		private void AssertSendAtEq(DateTime date)
		{
			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT SendAt FROM Messages";
				using (var reader = cmd.ExecuteReader())
				{
					int count = 0;
					while (reader.Read())
					{
						count += 1;
						var time = reader.GetDateTime(0);
						Assert.AreEqual(date, time);
					}
					Assert.AreEqual(2, count);
				}
			}
		}

		[Test]
		public void Can_move_all_failed_messages_in_batch_to_failed_messages()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());

			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.MarkAllInBatchAsFailed(send.Id, new Uri("queue://localhost/test"));

			outgoingMessageRepository.MoveUnderliverableMessagesToDeadLetterQueue(
				send.Id, new Uri("queue://localhost/test"), 1, new ArgumentException("foo"));

			using (var con = outgoingMessageRepository.CreateConnection())
			using (var cmd = con.CreateCommand())
			{
				cmd.CommandText = "SELECT LastException FROM FailedMessages";
				using (var reader = cmd.ExecuteReader())
				{
					int count = 0;
					while (reader.Read())
					{
						count += 1;
						var time = reader.GetString(0);
						Assert.AreEqual("System.ArgumentException: foo", time);
					}
					Assert.AreEqual(2, count);
				}

				cmd.CommandText = "SELECT COUNT(*) FROM Messages";
				cmd.Parameters.Clear();
				Assert.AreEqual(0, cmd.ExecuteScalar());
			}
		}
	}
}