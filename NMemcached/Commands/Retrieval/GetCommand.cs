using System.IO;
using NMemcached.Commands.Retrieval;
using NMemcached.Model;

namespace NMemcached.Commands.Retrieval
{
	public class GetCommand : AbstractRetrievalCommand
	{
		public GetCommand(Stream stream) : base(stream)
		{
		}

		public override void Execute()
		{
			foreach (var key in Keys)
			{
				var item = (CachedItem)Cache.Get(key);
				if (item == null)
					continue;
				if (item.ExpiresAt < SystemTime.Now())
					continue;
				Writer.WriteLine("VALUE {0} {1} {2}", item.Key, item.Flags, item.Buffer.Length);
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