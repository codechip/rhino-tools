using System;
using System.IO;
using System.Linq;
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
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");

			outgoingMessageRepository = new OutgoingMessageRepository("test", "test");
			new BerkeleyDbPhysicalStorage("test").CreateOutputQueue("test");
		}

		[Test]
		public void When_update_failure_count_will_increase_failure_count_for_all_items_in_batch_and_destination()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			
			AssertFailureCount(0);

			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://localhost/test"), 100, new Exception());

			AssertFailureCount(1);

			SystemTime.Now = () => new DateTime(2000, 1, 2);

			send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://localhost/test"), 100, new Exception());

			AssertFailureCount(2);
		}

		[Test]
		public void When_update_failure_count_will_update_send_at_time()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());

			DateTime expected = SystemTime.Now();
			AssertSendAtEq(expected);

			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://localhost/test"), 100, new Exception());
			expected = SystemTime.Now().AddSeconds(1);
			AssertSendAtEq(expected);

			// need to update so we will select them
			SystemTime.Now = () => new DateTime(2000,1,1,0,0,1); 

			send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://localhost/test"), 100, new Exception());
			expected = SystemTime.Now().AddSeconds(4);
			AssertSendAtEq(expected);
		}

		[Test]
		public void When_update_failure_count_will_update_send_at_time_in_increasing_intervals()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());

			DateTime expected = SystemTime.Now();
			AssertSendAtEq(expected);
			var secondsToAddToCurrentTime = new[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100};

			foreach (var i in secondsToAddToCurrentTime)
			{
				// need to update so we will select them
				SystemTime.Now = () => expected.AddSeconds(i);

				var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
				outgoingMessageRepository.ReturnedFailedBatchToQueue(
					send.Id, 
					new Uri("queue://localhost/test"), 
					100, 
					new Exception());

				expected = SystemTime.Now().AddSeconds(i);
				AssertSendAtEq(expected);
			}
		}


		private static void AssertFailureCount(int expectedFailureCountForEachMessage)
		{
			using (var repository = new OutgoingTestRepository("test"))
			{
				int count = 0;
				foreach (var message in repository.GetTransportMessages())
				{
					count += 1;
					Assert.AreEqual(expectedFailureCountForEachMessage, message.Message.FailureCount);
				}
				Assert.AreEqual(2,count);
			}
		}

		private static void AssertSendAtEq(DateTime date)
		{
			using (var repository = new OutgoingTestRepository("test"))
			{
				int msgs = 0;
				foreach (var message in repository.GetTransportMessages())
				{
					msgs +=1;
					Assert.AreEqual(date, message.SendAt);
				}
				Assert.AreEqual(2, msgs);
			}
		}

		[Test]
		public void Can_save_message()
		{
			outgoingMessageRepository.Save(new Uri("queue://foo/bar"), new QueueMessage());
		}

		[Test]
		public void Can_get_saved_message()
		{
			outgoingMessageRepository.Save(new Uri("queue://foo/bar"), new QueueMessage());

			MessageBatch send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.IsNotNull(send);
			Assert.AreEqual(1, send.DestinationBatches.Length);
		}

		[Test]
		public void When_reseting_batch_will_change_only_items_in_that_batch()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test1"),
				10, new Exception());

			using (var repository = new OutgoingTestRepository("test"))
			{
				var countInActive = repository.GetCountInActiveMessages();
				var countInBatches = repository.GetCountInBatches();
				Assert.AreEqual(1, countInActive);
				Assert.AreEqual(1, countInBatches);
			}
		}

		[Test]
		public void When_moving_messages_to_dead_letter_Will_also_save_exception_information()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test2"),
				0, new ArgumentException("foo"));

			using (var repository = new OutgoingTestRepository("test"))
			{
				var entry = repository.GetDeadLetters().Single();
				Assert.AreEqual("foo", entry.Exception.Message);
			}
		}

		[Test]
		public void When_moving_messages_to_dead_letter_Will_also_save_context_information()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test2"),
				0, new ArgumentException("foo"));

			using (var repository = new OutgoingTestRepository("test"))
			{
				var entry = repository.GetDeadLetters().Single();
				Assert.AreEqual(SystemTime.Now(), entry.FinalFailureAt);
				Assert.AreEqual(new Uri("queue://test/test2"), entry.Destination);
			}
		}

		[Test]
		public void When_reseting_batch_will_move_message_with_failure_count_over_max_to_dead_letter_queue()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test1"),
				0, new Exception());

			using (var repository = new OutgoingTestRepository("test"))
			{
				var countInActive = repository.GetCountInActiveMessages();
				var countInBatches = repository.GetCountInBatches();
				var countInDeadLetters = repository.GetCountInDeadLetters();
				Assert.AreEqual(0, countInActive);
				Assert.AreEqual(1, countInBatches);
				Assert.AreEqual(1, countInDeadLetters);
			}
		}

		[Test]
		public void When_purging_messages_will_remove_all_active_failed_and_dead_messages()
		{
			outgoingMessageRepository.Save(new Uri("queue://test/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://test/test2"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test2"), 1, new Exception());
			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://test/test1"), 0, new Exception());


			outgoingMessageRepository.PurgeAllMessages();

			using (var repository = new OutgoingTestRepository("test"))
			{
				var countInActive = repository.GetCountInActiveMessages();
				var countInBatches = repository.GetCountInBatches();
				var countInDeadLetters = repository.GetCountInDeadLetters();
				Assert.AreEqual(0, countInActive);
				Assert.AreEqual(0, countInBatches);
				Assert.AreEqual(0, countInDeadLetters);
			}

		}
	}
}