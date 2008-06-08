using System.IO;
using NMemcached.Commands.Storage;

namespace NMemcached.Commands.Storage
{
	public class SetCommand : AbstractStoreCommand
	{
		protected override void ExecuteCommand()
		{
			InsertItemToCache();
		}
	}
}