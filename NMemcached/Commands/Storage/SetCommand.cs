using System.IO;
using NMemcached.Commands.Storage;

namespace NMemcached.Commands.Storage
{
	public class SetCommand : AbstractStoreCommand
	{
		public SetCommand(Stream stream)
			: base(stream)
		{
		}

		protected override void ExecuteCommand()
		{
			InsertItemToCache();
		}
	}
}