using NMemcached.Commands;

namespace NMemcached.Extensions
{
	public static class CommandExtensions
	{
		public static void SendToClient(this AbstractCommand command, string msg)
		{
			if (command.NoReply)
				return;
			command.Writer.WriteLine(msg);
			command.Writer.Flush();
		}
	}
}