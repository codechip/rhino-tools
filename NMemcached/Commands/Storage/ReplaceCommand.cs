using NMemcached.Commands.Storage;
using NMemcached.Extensions;
using NMemcached.Model;

namespace NMemcached.Commands.Storage
{
	public class ReplaceCommand : AbstractStoreCommand
	{

		protected override void ExecuteCommand()
		{
			object o = Cache.Get(Key);
			if(o == null || o is BlockOperationOnItemTag)
			{
				this.SendToClient("NOT_STORED");
				RaiseFinishedExecuting();
				return;
			}

			InsertItemToCache();
		}
	}
}