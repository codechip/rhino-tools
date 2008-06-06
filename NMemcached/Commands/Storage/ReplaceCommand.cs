using System.IO;
using System.Web.Caching;
using NMemcached.Commands.Storage;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class ReplaceCommand : AbstractStoreCommand
	{
		public ReplaceCommand(Stream stream) : base(stream)
		{
		}

		protected override void ExecuteCommand()
		{
			object o = Cache.Get(Key);
			if(o == null || o is BlockOperationOnItemTag)
			{
				SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}

			InsertItemToCache();
		}
	}
}