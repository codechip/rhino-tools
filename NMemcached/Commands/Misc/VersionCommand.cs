using System;
using System.IO;

namespace NMemcached.Commands.Misc
{
	public class VersionCommand : AbstractCommand
	{
		public override void Execute()
		{
			Version version = typeof (VersionCommand).Assembly.GetName().Version;
			Writer.WriteLine("VERSION " + version);
			Writer.Flush();
		}

		public override bool Init(string[] args)
		{
			if (args.Length != 0)
				return ClientError("Version accepts no paramters");
			return true;
		}
	}
}