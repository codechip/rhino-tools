using System;
using Rhino.Queues.Extensions;

namespace Rhino.Queues
{
	[Serializable]
	public class SingleDestinationMessageBatch
	{
		public Uri Source{ get; set;}
		public Guid BatchId { get; set;  }
		public Uri Destination { get; set; }
		public QueueMessage[] Messages { get; set; }

		public string DestinationQueue
		{
			get { return Destination.ToQueueName(); }
		}

		public string SourceQueue
		{
			get { return Source.ToQueueName(); }
		}

		public SingleDestinationMessageBatch()
		{
			Messages = new QueueMessage[0];
		}
	}
}