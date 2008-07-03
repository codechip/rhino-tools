using System;
using log4net;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Accept_message_command_tests
	{
		[Test]
		public void When_asked_to_send_batch_will_find_approrpriate_queue_and_call_accept_message_on_it()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, }
			};

			var factory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var queue = MockRepository.GenerateStub<IQueueImpl>();
			factory.Stub(x => x.GetLocalQueue(destinationBatch.DestinationQueue)).Return(queue);

			var cmd = new AcceptMessagesCommand(factory, destinationBatch);
			cmd.Execute();

			queue.AssertWasCalled(x => x.AcceptMessages(msg));
		}

		[Test]
		public void Will_issue_log_if_not_found_appropriate_log()
		{
			var msg = new QueueMessage();
			var destinationBatch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost:9523/testQueue"),
				Messages = new[] { msg, }
			};

			var factory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var cmd = new AcceptMessagesCommand(factory, destinationBatch);
			using(var spy = new LogSpy(typeof(AcceptMessagesCommand)))
			{
				cmd.Execute();
				Assert.AreEqual(1, spy.Messages.Length);
			}
		}
	}
}