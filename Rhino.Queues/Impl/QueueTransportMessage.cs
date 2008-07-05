using System;

namespace Rhino.Queues.Impl
{
	[Serializable]
	public class QueueTransportMessage
	{
		public DateTime SendAt { get; set; }
		public QueueMessage Message { get; set; }
		public Uri Destination { get; set; }
	}
}