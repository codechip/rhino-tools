using System;
using System.IO;
using NMemcached.Util;

namespace NMemcached.Commands
{
	public abstract class AbstractCommand : CacheMixin, ICommand
	{
		public Stream Stream { get; private set; }
		public TextWriter Writer { get; private set; }
		public bool NoReply { get; protected set; }

		public string Name
		{
			get { return GetType().Name.Replace("Command", "").ToLowerInvariant(); }
		}

		#region ICommand Members

		public void SetContext(Stream stream)
		{
			Stream = stream;
			Writer = new StreamWriter(stream);			
		}

		#endregion

		public abstract bool Init(params string[] args);
		public abstract void Execute();
		public virtual event Action FinishedExecuting = delegate { };

		protected void RaiseFinishedExecuting()
		{
			FinishedExecuting();
		}

		protected bool ClientError(string message)
		{
			try
			{
				Writer.WriteLine("CLIENT_ERROR " + message);
				Writer.Flush();
			}
			catch
			{
				// not much to do if we are broken here
			}
			return false;
		}

		protected bool TryParseTime(string arg, out DateTime? dateTime)
		{
			dateTime = null;

			int exptime;
			if (int.TryParse(arg, out exptime) == false)
				return ClientError("Exptime should be an integer");

			if (exptime == 0)
			{
				return true;
			}
			if (exptime > TimeSpan.FromDays(30).TotalSeconds)
			{
				dateTime = new DateTime(1970, 1, 1).AddSeconds(exptime);
			}
			else
			{
				dateTime = SystemTime.Now().AddSeconds(exptime);
			}
			return true;
		}

		protected bool ParseOptionalNoReplyArgument(string[] args, int indexOfOptionalArgument, out bool noReply)
		{
			noReply = false;
			if (args.Length > indexOfOptionalArgument)
			{
				if (args[indexOfOptionalArgument] == "noreply")
					noReply = true;
				else
					return ClientError("Last argument was expected to be [noreply]");
			}
			return true;
		}
	}
}