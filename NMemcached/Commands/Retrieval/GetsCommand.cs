using System.IO;
using NMemcached.Commands.Retrieval;
using NMemcached.Model;

namespace NMemcached.Commands.Retrieval
{
	public class GetsCommand : AbstractRetrievalCommand
	{
		public GetsCommand(Stream stream)
			: base(stream)
		{
		}

		public override void Execute()
		{
			foreach (var key in Keys)
			{
				var item = Cache.Get(key) as CachedItem;
				if (item == null)
					continue;
				if(item.ExpiresAt < SystemTime.Now())
					continue;

				Writer.WriteLine("VALUE {0} {1} {2} {3}", item.Key, item.Flags, item.Buffer.Length, item.Timestamp);
				// this is needed so it will go directly to the stream, so we will get the output
				// in the expected order
				Writer.Flush();
				Stream.Write(item.Buffer, 0, item.Buffer.Length);
				Writer.WriteLine();
			}
			Writer.WriteLine("END");
			Writer.Flush();
			RaiseFinishedExecuting();
		}
	}
}