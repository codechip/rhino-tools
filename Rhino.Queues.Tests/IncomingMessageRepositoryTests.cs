using System;
using System.IO;
using BerkeleyDb;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class IncomingMessageRepositoryTests
	{
		private IncomingMessageRepository incomingMessageRepository;

		[SetUp]
		public void Setup()
		{
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");
			incomingMessageRepository = new IncomingMessageRepository("test","test");
			new BerkeleyDbPhysicalStorage("test").CreateInputQueue("test");
		}

		[Test]
		public void If_there_are_no_messages_should_return_null()
		{
			var message = incomingMessageRepository.GetEarliestMessage();
			Assert.IsNull(message);
		}

		[Test]
		public void When_there_are_two_messages_will_return_the_earliest_of_them()
		{
			var msg1 = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};
			msg1.Headers["mark"] = "1";

			var msg2 = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};
			msg2.Headers["mark"] = "2";

			incomingMessageRepository.Save(msg1);
			incomingMessageRepository.Save(msg2);

			var message = incomingMessageRepository.GetEarliestMessage();
			Assert.AreEqual("1", message.Headers["mark"]);
		}

		[Test]
		public void When_there_are_two_messages_will_return_the_earliest_of_them_and_when_calling_again_will_give_the_next_one()
		{
			var msg1 = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};
			msg1.Headers["mark"] = "1";

			var msg2 = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};
			msg2.Headers["mark"] = "2";

			incomingMessageRepository.Save(msg1);
			incomingMessageRepository.Save(msg2);

			var message = incomingMessageRepository.GetEarliestMessage();
			Assert.AreEqual("1", message.Headers["mark"]);

			message = incomingMessageRepository.GetEarliestMessage();
			Assert.AreEqual("2", message.Headers["mark"]);
		}

		[Test]
		public void When_purging_messages_will_remove_all_message_from_storage()
		{
			incomingMessageRepository.Save(new QueueMessage());

			incomingMessageRepository.PurgeAllMessages();

			Assert.IsNull(incomingMessageRepository.GetEarliestMessage());
		}

		[Test]
		public void When_getting_message_from_db_will_delete_it()
		{
			var msg1 = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};
			msg1.Headers["mark"] = "1";

			incomingMessageRepository.Save(msg1);

			var message = incomingMessageRepository.GetEarliestMessage();
			Assert.IsNotNull(message);
			using(var env = new BerkeleyDbEnvironment("test"))
			using(var queue = env.OpenQueue("test.queue"))
			{
				Assert.IsNull(queue.Consume());
			}
		}

		[Test]
		public void Can_save_and_load_message()
		{
			var msg = new QueueMessage
			{
				Body = new byte[] { 1, 2, 3, 4 },
				CorrelationId = Guid.NewGuid(),
			};

			msg.Headers["customer-header"] = "bhal";

			incomingMessageRepository.Save(msg);
			var msg2 = incomingMessageRepository.GetEarliestMessage();

			Assert.AreEqual(msg.Id, msg2.Id);
			Assert.AreEqual(msg.CorrelationId, msg2.CorrelationId);
			CollectionAssert.AreEqual(msg.Body, msg2.Body);
			Assert.AreEqual(msg.Headers["custom-header"], msg2.Headers["custom-header"]);
		}
	}
}