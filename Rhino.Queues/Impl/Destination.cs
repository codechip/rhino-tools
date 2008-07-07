using System;

namespace Rhino.Queues.Impl
{
	[Serializable]
	public class Destination
	{
		public string Queue { get; set; }
		public string Server { get; set; }

		public Destination()
		{
			
		}

		public Destination(IQueueFactory queueFactory, string destination)
		{
			if (string.IsNullOrEmpty(destination))
				throw new ArgumentException("Destination cannot be null or empty");

			var parts = destination.Split(new[] { '@' }, 2);
			Queue = parts[0];

			Server = parts.Length == 2 ? parts[1] : queueFactory.Name;
		}
	}
}