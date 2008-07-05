using System;
using System.IO;
using BerkeleyDb;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class OutgoingMessageRepository_reset_batch_tests
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
		public void Can_reset_batch_by_destination_and_batch_id()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			//associcate everything with a batch
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ReturnedFailedBatchToQueue(send.Id, new Uri("queue://localhost/test1"),100, new Exception());
			SystemTime.Now = () => new DateTime(2000, 1, 2);
			// should get the test1 messages in a new batch
			send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(1, send.DestinationBatches.Length);
			Assert.AreEqual(new Uri("queue://localhost/test1"), send.DestinationBatches[0].Destination);
			Assert.AreEqual(2, send.DestinationBatches[0].Messages.Length);
		}

		[Test]
		public void Can_delete_batch_by_destination_and_batch_id()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			//associcate everything with a batch
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(2, send.DestinationBatches.Length);

			outgoingMessageRepository.RemoveSuccessfulBatch(send.Id, new Uri("queue://localhost/test1"));

			outgoingMessageRepository.ResetAllBatches();

			//associcate everything with a batch
			send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(1, send.DestinationBatches.Length);
		}

		[Test]
		public void Can_reset_all_batches()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			//associcate everything with a batch
			outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ResetAllBatches();

			// should get the test1 messages in a new batch
			var send = outgoingMessageRepository.GetBatchOfMessagesToSend();
			Assert.AreEqual(2, send.DestinationBatches.Length);
			//force a sort, for the test
			Array.Sort(send.DestinationBatches, (x, y) => x.Destination.AbsolutePath.CompareTo(y.Destination.AbsolutePath));

			Assert.AreEqual(new Uri("queue://localhost/test1"), send.DestinationBatches[0].Destination);
			Assert.AreEqual(new Uri("queue://localhost/test2"), send.DestinationBatches[1].Destination);

			Assert.AreEqual(2, send.DestinationBatches[0].Messages.Length);
			Assert.AreEqual(2, send.DestinationBatches[1].Messages.Length);
		}

		[Test]
		public void When_reseting_all_batches_will_set_send_time_to_current_time()
		{
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test1"), new QueueMessage());

			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());
			outgoingMessageRepository.Save(new Uri("queue://localhost/test2"), new QueueMessage());

			//associcate everything with a batch
			outgoingMessageRepository.GetBatchOfMessagesToSend();

			outgoingMessageRepository.ResetAllBatches();
			
			using (var env = new BerkeleyDbEnvironment("test"))
			using (var tree = env.OpenTree("test.tree"))
			using (var queue = env.OpenQueue("test.queue"))
			{
				foreach (var message in queue.SelectFromAssociation<QueueTransportMessage>(tree))
				{
					Assert.AreEqual(message.SendAt, SystemTime.Now());
				}
			}
		}
	}
}