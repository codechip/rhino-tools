using System;

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
			get { return Destination.LocalPath.Substring(1); }
		}

		public string SourceQueue
		{
			get { return Source.LocalPath.Substring(1); }
		}

		public SingleDestinationMessageBatch()
		{
			Messages = new QueueMessage[0];
		}
	}
}