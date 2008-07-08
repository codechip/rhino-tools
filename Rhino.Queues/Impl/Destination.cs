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

		public Destination(string destination)
			: this(destination, null)
		{

		}

		public Destination(string destination, string localServer)
		{
			if (string.IsNullOrEmpty(destination))
				throw new ArgumentException("Destination cannot be null or empty");

			var parts = destination.Split(new[] { '@' }, 2);
			Queue = parts[0];

			Server = parts.Length == 2 ? parts[1] : localServer;
			if (string.IsNullOrEmpty(Server))
			{
				throw new ArgumentException(
					"Could not figure out what server is the destination, and no local server was passed");
			}
		}

		public override string ToString()
		{
			return Queue + "@" + Server;
		}

		public static implicit operator string(Destination d)
		{
			return d.ToString();
		}
	}
}