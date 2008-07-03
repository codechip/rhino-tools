using System;
using System.IO;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class OutgoingMessageRepositoryTests_batches_tests
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
		public void When_there_are_no_items_in_queue_will_give_empty_batch()
		{
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(0, send.DestinationBatches.Length);
		}

		[Test]
		public void When_there_is_one_item_in_queue_will_give_batch_of_single_destination_with_single_item()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(1, send.DestinationBatches.Length);
			Assert.AreEqual(new Uri("queue://localhost/test"), send.DestinationBatches[0].Destination);
			Assert.AreEqual(1, send.DestinationBatches[0].Messages.Length);
		}

		[Test]
		public void Will_return_no_items_if_sendat_date_is_later_than_current_time()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());

			SystemTime.Now = () => new DateTime(1999, 1, 1);
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(0, send.DestinationBatches.Length);
		}

		[Test]
		public void Will_return_no_items_if_sendat_date_is_before_than_current_time()
		{
			SystemTime.Now = () => new DateTime(2999, 1, 1);
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(1, send.DestinationBatches.Length);
		}


		[Test]
		public void After_getting_batch_there_are_no_messages_get_from_queue()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(1, send.DestinationBatches.Length);
			send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(0, send.DestinationBatches.Length);
		}

		[Test]
		public void When_there_are_more_than_100_messages_will_return_first_100()
		{
			for (int i = 0; i < 200; i++)
			{
				outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
				outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			}
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			int total = 0;
			foreach (var batch in send.DestinationBatches)
			{
				total += batch.Messages.Length;
			}
			Assert.AreEqual(100, total);
		}

		[Test]
		public void When_there_are_several_items_for_same_destination_in_queue_will_group_all_related_to_same_batch()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test3"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test3"), new QueueMessage());


			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(3, send.DestinationBatches.Length);
			//force a sort, for the test
			Array.Sort(send.DestinationBatches, (x, y) => x.Destination.AbsolutePath.CompareTo(y.Destination.AbsolutePath));

			Assert.AreEqual("queue://localhost/test1", send.DestinationBatches[0].Destination.ToString());
			Assert.AreEqual("queue://localhost/test2", send.DestinationBatches[1].Destination.ToString());
			Assert.AreEqual("queue://localhost/test3", send.DestinationBatches[2].Destination.ToString());

			Assert.AreEqual(2, send.DestinationBatches[0].Messages.Length);
			Assert.AreEqual(2, send.DestinationBatches[1].Messages.Length);
			Assert.AreEqual(2, send.DestinationBatches[2].Messages.Length);
		}

		[Test]
		public void When_there_are_several_items_for_same_destination_in_queue_will_group_all_related_to_same_batch_ignoring_case()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test3"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test3"), new QueueMessage());


			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(3, send.DestinationBatches.Length);
			//force a sort, for the test
			Array.Sort(send.DestinationBatches, (x, y) => x.Destination.ToString().CompareTo(y.Destination.ToString()));
			StringAssert.AreEqualIgnoreCase("queue://localhost/test1", send.DestinationBatches[0].Destination.ToString());
			StringAssert.AreEqualIgnoreCase("queue://localhost/test2", send.DestinationBatches[1].Destination.ToString());
			StringAssert.AreEqualIgnoreCase("queue://localhost/test3", send.DestinationBatches[2].Destination.ToString());

			Assert.AreEqual(2, send.DestinationBatches[0].Messages.Length);
			Assert.AreEqual(2, send.DestinationBatches[1].Messages.Length);
			Assert.AreEqual(2, send.DestinationBatches[2].Messages.Length);
		}

		[Test]
		public void BatchId_on_message_batch_and_on_destination_batch_should_be_identical()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test"), new QueueMessage());
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			foreach (var batch in send.DestinationBatches)
			{
				Assert.AreEqual(send.Id, batch.BatchId);
			}
		}

	}
}