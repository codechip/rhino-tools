using System.IO;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	/// <summary>
	/// CAS == Compare and Store
	/// </summary>
	public class CasCommand : AbstractStoreCommand
	{
		public long Timestamp { get; private set; }

		public CasCommand(Stream stream)
			: base(stream)
		{
		}

		public override bool Init(string[] args)
		{
			if (args.Length < 5 || args.Length > 6)
				return ClientError("Expected to get '" + Name + " <key> <flags> <exptime> <bytes> <cas unqiue> [noreply]'");

			if (ParseStoreArguments(args) == false)
				return false;

			long timestamp;
			if (long.TryParse(args[4], out timestamp) == false)
				return ClientError("cas value should be numeric");
			Timestamp = timestamp;

			bool noReply;
			if (ParseOptionalNoReplyArgument(args, 5, out noReply) == false)
				return false;
			NoReply = noReply;

			return true;
		}

		protected override void ExecuteCommand()
		{
			object cachedItem = Cache.Get(Key);
			if(cachedItem is BlockOperationOnItemTag)
			{
				SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}

			var item = cachedItem as CachedItem;
			if (item == null)
			{
				SendToClient("NOT_FOUND");
				RaiseFinishedExecuting();
				return;
			}
			lock (item)
			{
				if (item.Timestamp != Timestamp)
				{
					SendToClient("EXISTS");
					RaiseFinishedExecuting();
					return;
				}
				InsertItemToCache();
			}
		}
	}
}