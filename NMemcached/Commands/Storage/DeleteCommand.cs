using System;
using System.IO;
using System.Web.Caching;
using NMemcached.Extensions;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class DeleteCommand : AbstractCommand
	{
		public string Key { get; private set; }

		public DateTime? BlockedFromUpdatingUntil { get; private set; }

		public DeleteCommand(Stream stream)
			: base(stream)
		{
		}

		public override void Execute()
		{
			object removedItem;
			if (BlockedFromUpdatingUntil != null)
			{
				var doNotModifyTag = new BlockOperationOnItemTag(BlockedFromUpdatingUntil.Value);
				removedItem = Cache.Get(Key);
				Cache.Insert(Key, doNotModifyTag, null, BlockedFromUpdatingUntil.Value,
				             NoSlidingExpiration, CacheItemPriority.High, null);
				if (removedItem == null)//it wasn't in the cache in first place
					Cache.Remove(Key);
			}
			else
			{
				removedItem = Cache.Remove(Key);
			}
			if (removedItem != null)
				this.SendToClient("DELETED");
			else
				this.SendToClient("NOT_FOUND");
			RaiseFinishedExecuting();
		}

		public override bool Init(string[] args)
		{
			if (args.Length < 1 || args.Length > 3)
				return ClientError("Expected 'delete <key> [<time>] [noreply]'");

			Key = args[0];

			if (args.Length == 1)
				return true;

			DateTime? dateTime;
			if (TryParseTime(args[1], out dateTime) == false)
				return false;
			BlockedFromUpdatingUntil = dateTime;

			bool noReply;
			if (ParseOptionalNoReplyArgument(args, 2, out noReply) == false)
				return false;
			NoReply = noReply;

			return true;
		}
	}
}