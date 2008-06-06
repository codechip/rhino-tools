using System;
using System.IO;

namespace NMemcached.Util
{
	internal class AyncStreamReader
	{
		private readonly Stream stream;
		private readonly byte[] buffer;
		private Action<Exception> action;
		private int currentBufferIndex;

		public AyncStreamReader(Stream stream, byte[] buffer)
		{
			this.stream = stream;
			this.buffer = buffer;
		}

		public void OnReadData(IAsyncResult ar)
		{
			try
			{
				int read = stream.EndRead(ar);
				if (read == 0)//couldn't find more data 
				{
					action(null);
					return;
				}
				currentBufferIndex += read;
				if (currentBufferIndex < buffer.Length)
				{
					stream.BeginRead(buffer, currentBufferIndex, buffer.Length - currentBufferIndex, OnReadData, null);
					return;
				}
				action(null);
			}
			catch (Exception e)
			{
				action(e);
			}
		}

		public void Read(Action<Exception> actionToExecuteWhenDoneReading)
		{
			action = actionToExecuteWhenDoneReading;
			stream.BeginRead(buffer, 0, buffer.Length, OnReadData, null);
		}
	}
}