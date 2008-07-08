using System;
using System.Collections.Specialized;
using Rhino.Queues.Impl;

namespace Rhino.Queues
{
	[Serializable]
	public class Message
	{
		public Message()
		{
			Headers = new NameValueCollection();
		}

		public NameValueCollection Headers { get; set; }
		public DateTime SentAt { get; set; }
		public object Value { get; set; }
		public Destination Source { get; set; }
		public Destination Destination { get; set; }
	}
}