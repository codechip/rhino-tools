using System.IO;
using System.Text;
using NMemcached.Model;

namespace NMemcached.Commands.Modifications
{
	public abstract class AbstractArithmeticOperation : AbstractCommand
	{
		protected AbstractArithmeticOperation(Stream stream) : base(stream)
		{
		}

		public ulong Value { get; private set; }
		public string Key { get; private set; }

		public override void Execute()
		{
			var item = (CachedItem) Cache.Get(Key);
			if(item==null)
			{
				SendToClient("NOT_FOUND");
				RaiseFinishedExecuting();
				return;
			}
			ulong value = 0;
			lock (item)
			{
				try
				{
					string str = Encoding.ASCII.GetString(item.Buffer);
					value = ulong.Parse(str);
				}
				catch
				{
				}
				value = ArithmeticOperation(value);
				item.Buffer = Encoding.ASCII.GetBytes(value.ToString());
			}
			SendToClient(value.ToString());
			RaiseFinishedExecuting();
		}

		protected abstract ulong ArithmeticOperation(ulong cachedValue);

		public override bool Init(string[] args)
		{
			if(args.Length < 2 || args.Length> 3)
				return ClientError("Expected to get '" + Name + " <key> <values> [noreply]'");

			Key = args[0];
			
			ulong value;
			if (ulong.TryParse(args[1], out value) == false)
				return ClientError("Value should be a numeric value");

			Value = value;

			bool noReply;
			if(ParseOptionalNoReplyArgument(args, 2, out noReply)==false)
				return false;
			
			NoReply = noReply;

			return true;
		}

		private void SendToClient(string msg)
		{
			if (NoReply)
				return;
			Writer.WriteLine(msg);
			Writer.Flush();
		}
	}
}