using System;
using System.IO;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class PrependCommand : AbstractStoreCommand
	{
		public PrependCommand(Stream stream) : base(stream)
		{
		}

		protected override void ExecuteCommand()
		{
			var cachedItem = (CachedItem)Cache.Get(Key);
			if (cachedItem == null)
			{
				SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}
			lock (cachedItem)
			{
				byte[] oldBuffer = cachedItem.Buffer;
				cachedItem.Buffer = new byte[cachedItem.Buffer.Length + BytesCount];

				Array.Copy(Buffer, 0, cachedItem.Buffer, 0, Buffer.Length);
				Array.Copy(oldBuffer, 0, cachedItem.Buffer, BytesCount, oldBuffer.Length);
			}
			SendToClient("STORED");
			RaiseFinishedExecuting();
		}
	}
}