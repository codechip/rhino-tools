using System.IO;
using System.Web.Caching;
using NMemcached.Commands.Storage;

namespace NMemcached.Commands.Storage
{
	public class AddCommand : AbstractStoreCommand
	{
		public AddCommand(Stream stream)
			: base(stream)
		{
		}

		protected override void ExecuteCommand()
		{
			if (Cache.Get(Key) != null)
			{
				SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}

			InsertItemToCache();
		}
	}
}