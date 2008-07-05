using System;
using System.Runtime.Serialization;

namespace Rhino.Queues.Impl
{
	[Serializable]
	public class FailedQueueMessage
	{
		public DateTime FinalFailureAt { get; set; }
		public QueueMessage Message { get; set; }
		public Uri Destination { get; set; }
		private Exception exception;

		public Exception Exception
		{
			get { return exception; }
			set
			{
				if (exception != null && exception.GetType().IsSerializable == false)//yuck!
				{
					// we want to seralize that, so we will just save the text.
					value = new UnserializableProxyException(value.ToString());
				}
				exception = value;
			}
		}

		[Serializable]
		public class UnserializableProxyException : Exception
		{
			public UnserializableProxyException()
			{
			}

			public UnserializableProxyException(string message) : base(message)
			{
			}

			public UnserializableProxyException(string message, Exception inner) : base(message, inner)
			{
			}

			protected UnserializableProxyException(
				SerializationInfo info,
				StreamingContext context) : base(info, context)
			{
			}
		}
	}
}