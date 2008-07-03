using System;
using System.Collections.Specialized;

namespace Rhino.Queues
{
	[Serializable]
	public class QueueMessage
	{
		private readonly NameValueCollection headers = new NameValueCollection();
		private Guid id = Guid.NewGuid();

		public Uri Source { get; set;  }

		public Guid CorrelationId { get; set; }

		public NameValueCollection Headers
		{
			get { return headers; }
		}

		public Guid Id
		{
			get { return id; }
			internal set { id = value; }
		}

		public byte[] Body { get; set; }
	}
}