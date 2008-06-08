using System.IO;
using System.Web.Caching;
using NMemcached.Commands.Storage;
using NMemcached.Extensions;

namespace NMemcached.Commands.Storage
{
	public class AddCommand : AbstractStoreCommand
	{
		protected override void ExecuteCommand()
		{
			if (Cache.Get(Key) != null)
			{
				this.SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}

			InsertItemToCache();
		}
	}
}