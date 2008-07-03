using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Queue_remote_interactions
	{
		private Queue queue;

		private readonly QueueMessage msg = new QueueMessage
		{
			Body = new byte[] { 1, 2, 3, 4 }
		};

		private IIncomingMessageRepository stubbedIncomingMsgsRepos;
		private IOutgoingMessageRepository stubbedOutgoingMsgsRepos;

		[SetUp]
		public void Setup()
		{
			stubbedOutgoingMsgsRepos = MockRepository.GenerateStub<IOutgoingMessageRepository>();
			stubbedIncomingMsgsRepos = MockRepository.GenerateStub<IIncomingMessageRepository>();
			queue = new Queue(new Uri("queue://my/test"), stubbedOutgoingMsgsRepos, stubbedIncomingMsgsRepos);
		}

		[Test]
		public void When_calling_send_will_save_to_outgoing_repository()
		{
			queue.Send(msg);

			stubbedOutgoingMsgsRepos.AssertWasCalled(x=>x.Save(queue.Url, msg));
		}
	}
}