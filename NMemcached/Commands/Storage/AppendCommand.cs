using System;
using System.IO;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class AppendCommand : AbstractStoreCommand
	{
		public AppendCommand(Stream stream) : base(stream)
		{
		}

		protected override void ExecuteCommand()
		{
			var cachedItem = (CachedItem)Cache.Get(Key);
			if(cachedItem==null)
			{
				SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}
			lock(cachedItem)
			{
				int oldBufferLength = cachedItem.Buffer.Length;
				byte[] buffer = cachedItem.Buffer;
				Array.Resize(ref buffer, oldBufferLength + BytesCount);
				Array.Copy(Buffer, 0, buffer, oldBufferLength, Buffer.Length);
				cachedItem.Buffer = buffer;
			}
			SendToClient("STORED");
			RaiseFinishedExecuting();
		}
	}
}