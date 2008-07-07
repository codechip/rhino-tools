using System;

namespace Rhino.Queues.Impl
{
	[Serializable]
	public class TransportMessage
	{
		public Guid Id { get; set; }

		public Destination Destination { get; set; }

		public object Message { get; set; }

		public DateTime SentAt { get; set; }

		public TransportMessage()
		{
			Id = Guid.NewGuid();
		}
	}
}