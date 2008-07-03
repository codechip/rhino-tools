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
		public void When_notifying_queue_about_batch_failure_will_mark_batch_as_failed()
		{
			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost/test")
			};
			queue.FailedToTransfer(batch, new Exception());
			stubbedOutgoingMsgsRepos.Stub(x => x.MarkAllInBatchAsFailed(batch.BatchId, batch.Destination));
		}

		[Test]
		public void When_notifying_queue_about_batch_failure_will_move_underliverable_messages_to_dead_letter_queue()
		{
			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost/test")
			};
			var e = new Exception();
			queue.FailedToTransfer(batch, e);
			stubbedOutgoingMsgsRepos.Stub(x => x.MoveUnderliverableMessagesToDeadLetterQueue(batch.BatchId, batch.Destination, 100, e));
		}

		[Test]
		public void When_notifying_queue_about_batch_failure_will_reset_batch()
		{
			var batch = new SingleDestinationMessageBatch
			{
				BatchId = Guid.NewGuid(),
				Destination = new Uri("queue://localhost/test")
			};
			var e = new Exception();
			queue.FailedToTransfer(batch, e);
			stubbedOutgoingMsgsRepos.Stub(x => x.ResetBatch(batch.BatchId, batch.Destination));
		}
	}
}