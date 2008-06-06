using System.IO;

namespace NMemcached.Commands.Retrieval
{
	public abstract class AbstractRetrievalCommand : AbstractCommand
	{
		public string[] Keys { get; private set; }

		protected AbstractRetrievalCommand(Stream stream) : base(stream)
		{
		}

		public override bool Init(string[] args)
		{
			if (args.Length == 0)
				return ClientError("At least one key should be specified");

			Keys = args;
			return true;
		}
	}
}