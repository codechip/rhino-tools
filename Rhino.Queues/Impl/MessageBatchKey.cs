using System;

namespace Rhino.Queues.Impl
{
	[Serializable]
	public class MessageBatchKey
	{
		public Guid Key;
		public Uri Destination;

		public MessageBatchKey(SingleDestinationMessageBatch batch)
		{
			Key = batch.BatchId;
			Destination = batch.Destination;
		}

		public MessageBatchKey(Guid key, Uri destination)
		{
			Key = key;
			Destination = destination;
		}
	}
}