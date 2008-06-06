using System;
using System.Collections;
using System.IO;
using NMemcached.Model;

namespace NMemcached.Commands.Misc
{
	public class FlushAllCommand : AbstractCommand
	{
		public FlushAllCommand(Stream stream) : base(stream)
		{
		}

		public override void Execute()
		{
			if(DelayFlushUntil==null)
			{
				ClearCache();
				SendToClient("OK");
				RaiseFinishedExecuting();
				return;
			}
			foreach (DictionaryEntry de in Cache)
			{
				CachedItem cachedItem = (CachedItem) de.Value;
				cachedItem.ExpiresAt = DelayFlushUntil.Value;
			}
			SendToClient("OK");
			RaiseFinishedExecuting();
		}

		public override bool Init(string[] args)
		{
			if (args.Length == 0)
				return true;

			if (args.Length > 2)
				return ClientError("Expected 'flush_all [delay] [noreply]'");

			DateTime? dateTime;
			if(TryParseTime(args[0],out dateTime)==false)
				return false;

			DelayFlushUntil = dateTime;

			bool noReply;
			if(ParseOptionalNoReplyArgument(args, 1, out noReply)==false)
				return false;
			NoReply = noReply;
			return true;
		}

		protected void SendToClient(string msg)
		{
			if (NoReply)
				return;

			Writer.WriteLine(msg);
			Writer.Flush();
		}

		public DateTime? DelayFlushUntil { get; private set; }
	}
}