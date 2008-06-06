using System;
using System.IO;
using System.Text;

namespace NMemcached.Util
{
	public class LineReader
	{
		private readonly Stream stream;
		private bool lastCharWasCr;
		StringBuilder sb = new StringBuilder();
		readonly byte[] buffer = new byte[1];
		private Action<string, Exception> action;
		public LineReader(Stream stream)
		{
			this.stream = stream;
		}

		public void Read(Action<string, Exception> actionToExecuteWhenDone)
		{
			this.action = actionToExecuteWhenDone;

			stream.BeginRead(buffer, 0, buffer.Length, OnReadData, null);
		}

		public void OnReadData(IAsyncResult ar)
		{
			try
			{
				stream.EndRead(ar);
				if (lastCharWasCr)
				{
					if (buffer[0] == '\n')
					{
						action(sb.ToString(), null);
						return;
					}
					sb.Append('\r');
				}
				lastCharWasCr = buffer[0] == '\r';
				if(lastCharWasCr==false)
					sb.Append(Convert.ToChar(buffer[0]));
				stream.BeginRead(buffer, 0, buffer.Length, OnReadData, null);
			}
			catch (Exception e)
			{
				action(null, e);
			}
		}

	}
}