using System;
using System.IO;

namespace NMemcached.Commands.Misc
{
	public class QuitCommand : AbstractCommand
	{
		private readonly Action quitAction;

		public QuitCommand(Action quitAction)
		{
			this.quitAction = quitAction;
		}

		public override void Execute()
		{
			quitAction();
		}

		public override bool Init(string[] args)
		{
			if (args.Length != 0)
				return ClientError("Quit accepts no paramters");
			return true;
		}
	}
}