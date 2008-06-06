using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NMemcached.Util
{
	public class LineReader
	{
		private readonly Stream stream;
		private bool lastCharWasCr;
		private readonly List<byte> buffer = new List<byte>();
		private Action<string, Exception> actionToExecuteWhenDone;
		private readonly byte[] initialBuffer = new byte[1];

		public LineReader(Stream stream)
		{
			this.stream = stream;
		}

		public void Read(Action<string, Exception> action)
		{
			actionToExecuteWhenDone = action;
			// we start with an async mode so we wouldn't have to deal with hanging the thread on read
			// or with stack overflow when we call back from it.
			stream.BeginRead(initialBuffer, 0, initialBuffer.Length, ReadToEndOfLineInSyncManner, null);
		}

		private void ReadToEndOfLineInSyncManner(IAsyncResult ar)
		{
			try
			{
				int read = stream.EndRead(ar);
				if(read==0)
					return;

				int readByte = initialBuffer[0];
				while (readByte != -1)
				{
					if (lastCharWasCr)
					{
						if (readByte == '\n')
						{
							break;
						}
						buffer.Add((byte)'\r');
					}
					lastCharWasCr = readByte == '\r';
					if (lastCharWasCr == false)
						buffer.Add((byte)readByte);
					readByte = stream.ReadByte();
				}
				actionToExecuteWhenDone(
					Encoding.ASCII.GetString(buffer.ToArray())
					, null);
			}
			catch (Exception e)
			{
				actionToExecuteWhenDone(null, e);
			}
		}
	}
}