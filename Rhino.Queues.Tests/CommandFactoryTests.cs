using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Commands;
using Rhino.Queues.Extensions;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class CommandFactoryTests
	{
		private IQueueFactoryImpl stubbedQueueFactory;
		[SetUp]
		public void Setup()
		{
			stubbedQueueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
		}

		[Test]
		public void When_creating_command_for_local_batch_will_create_accept_message_command()
		{
			var commandFactory = new CommandFactory(stubbedQueueFactory);
			stubbedQueueFactory.Stub(x => x.IsLocal(Arg<Uri>.Is.Anything))
				.Return(true);
			ICommand cmd = commandFactory.CreateCommand(new SingleDestinationMessageBatch());
			Assert.IsTrue(
				cmd is AcceptMessagesCommand
				);
		}

		[Test]
		public void When_creating_command_for_remote_batch_will_create_send_to_remote_command()
		{
			var commandFactory = new CommandFactory(stubbedQueueFactory);
			var localQueue = new Uri("queue://localhost/test");
		
			stubbedQueueFactory.Stub(x => x.IsLocal(Arg<Uri>.Is.Anything))
				.Return(false);

			stubbedQueueFactory.Stub(x => x.GetLocalQueue(localQueue.ToQueueName()))
				.Return(MockRepository.GenerateStub<IQueueImpl>());

			ICommand cmd = commandFactory.CreateCommand(new SingleDestinationMessageBatch{Source = localQueue});
			Assert.IsTrue(
				cmd is SendToRemoteServerCommand
				);
		}
	}
}