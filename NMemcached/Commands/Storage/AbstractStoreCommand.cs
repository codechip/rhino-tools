using System;
using System.IO;
using NMemcached.Extensions;
using NMemcached.Model;
using NMemcached.Util;

namespace NMemcached.Commands.Storage
{
	public abstract class AbstractStoreCommand : AbstractCommand
	{
		private readonly byte[] crLfBuffer = new byte[2];

		protected byte[] Buffer { get; private set; }

		public string Key { get; private set; }

		public uint Flags { get; private set; }

		public DateTime ExpiresAt { get; private set; }

		public int BytesCount { get; private set; }

		#region ICommand Members

		public override bool Init(string[] args)
		{
			if (args.Length < 4 || args.Length > 5)
				return ClientError("Expected to get '" + Name + " <key> <flags> <exptime> <bytes> [noreply]'");

			if (ParseStoreArguments(args) == false)
				return false;

			bool noReply;
			if (ParseOptionalNoReplyArgument(args, 4, out noReply) == false)
				return false;
			NoReply = noReply;

			return true;
		}

		protected bool ParseStoreArguments(string[] args)
		{
			Key = args[0];
			if (Key.IsEmpty())
				return ClientError("Key cannot be empty");
			if (Key.Length > 250)
				return ClientError("Key cannot be larger than 250 characters");

			uint flags;
			if (uint.TryParse(args[1], out flags) == false)
				return ClientError("Flags should be an 32 bits integer");
			Flags = flags;

			DateTime? expiry;
			if (TryParseTime(args[2], out expiry) == false)
				return false;
			ExpiresAt = expiry ?? SystemTime.Now().AddYears(100);

			int bytes;
			if (int.TryParse(args[3], out bytes) == false)
				return ClientError("Bytes should be a numeric value");
			if (bytes < 0)
				return ClientError("Bytes cannot be negative");

			BytesCount = bytes;

			return true;
		}

		public override void Execute()
		{
			Buffer = new byte[BytesCount];
			new AyncStreamReader(Stream, Buffer)
				.Read(ReadCrLfPair);
		}

		private void ReadCrLfPair(Exception e)
		{
			if (e != null)
				ExecuteCommand(e);
			try
			{
				new AyncStreamReader(Stream, crLfBuffer).Read(ExecuteCommand);
			}
			catch (Exception ex)
			{
				ExecuteCommand(ex);
			}
		}

		private void ExecuteCommand(Exception e)
		{
			if (e != null)
			{
				ClientError("Could not read data from stream, " + e.Message);
				RaiseFinishedExecuting();
				return;
			}
			if (DataEndsWithLineBreak() == false)
			{
				ClientError("Data section should end with a line break (\\r\\n)");
				RaiseFinishedExecuting();
				return;
			}
			ExecuteCommand();
		}

		protected abstract void ExecuteCommand();

		private bool DataEndsWithLineBreak()
		{
			if (crLfBuffer[0] != '\r')
				return false;
			if (crLfBuffer[1] != '\n')
				return false;
			return true;
		}

		#endregion

		protected void InsertItemToCache()
		{
			var cachedItem = new CachedItem { Key = Key, Buffer = Buffer, Flags = Flags, ExpiresAt = ExpiresAt};
			Cache.Insert(Key, cachedItem, null, ExpiresAt, NoSlidingExpiration);

			this.SendToClient("STORED");

			RaiseFinishedExecuting();
		}
	}
}