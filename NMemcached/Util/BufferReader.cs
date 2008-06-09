using System;
using System.IO;

namespace NMemcached.Util
{
	internal class BufferReader
	{
		private readonly Stream stream;
		private readonly byte[] buffer;

		public BufferReader(Stream stream, byte[] buffer)
		{
			this.stream = stream;
			this.buffer = buffer;
		}

		public void Read(Action<Exception> actionToExecuteWhenDoneReading)
		{
			try
			{
				int currentBufferIndex = 0;
				var bytesRead = -1;
				while(bytesRead!=0 && currentBufferIndex < buffer.Length)
				{
					bytesRead = stream.Read(buffer, currentBufferIndex, buffer.Length - currentBufferIndex);
					currentBufferIndex += bytesRead;
				}
				actionToExecuteWhenDoneReading(null);
			}
			catch (Exception e)
			{
				actionToExecuteWhenDoneReading(e);
			}
		}
	}
}