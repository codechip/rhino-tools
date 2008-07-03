using System;

namespace Rhino.Queues.Impl
{
	public interface IQueueImpl : ILocalQueue
	{
		void AcceptMessages(params QueueMessage[] msgs);
		void FailedToTransfer(SingleDestinationMessageBatch batch, Exception e);
		void SuccessfullyTransfered(SingleDestinationMessageBatch batch);
		void PurgeAllMessages();
	}
}