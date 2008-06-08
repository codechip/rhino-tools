using System;
using System.IO;
using NMemcached.Commands.Storage;
using NMemcached.Extensions;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class AppendCommand : AbstractStoreCommand
	{

		protected override void ExecuteCommand()
		{
			var cachedItem = Cache.Get(Key) as CachedItem;
			if(cachedItem==null)
			{
				this.SendToClient("NOT_STORED");
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
			this.SendToClient("STORED");
			RaiseFinishedExecuting();
		}
	}
}