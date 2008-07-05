using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Queue_notifications_tests
	{
		private Queue queue;

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
		public void When_notifying_queue_about_successful_batch_will_delete_batch_from_repository()
		{
			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost/test")
			};
			queue.SuccessfullyTransfered(batch);
			stubbedOutgoingMsgsRepos.Stub(x => x.RemoveSuccessfulBatch(batch.BatchId, batch.Destination));
		}

		[Test]
		public void When_notifying_queue_about_batch_failure_will_return_failed_batches_to_queue()
		{
			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost/test")
			};
			queue.FailedToTransfer(batch, new Exception());
			stubbedOutgoingMsgsRepos.AssertWasCalled(
				x => x.ReturnedFailedBatchToQueue(
					Arg<Guid>.Is.Anything, Arg<Uri>.Is.Anything, Arg<int>.Is.Anything, Arg<Exception>.Is.Anything));
		}
	}
}