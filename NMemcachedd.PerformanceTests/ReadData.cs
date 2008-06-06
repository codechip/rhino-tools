using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NMemcachedd.PerformanceTests
{
	public class ReadData
	{
		readonly byte[] buffer = new byte[2048];
		private Stream stream;
		private readonly Func<int> increment;
		private readonly TcpClient client;
		private readonly ManualResetEvent wait;

		public ReadData(TcpClient client, Func<int> increment, ManualResetEvent wait)
		{
			this.client = client;
			this.wait = wait;
			this.increment = increment;
		}

		public void BeginReading(IAsyncResult ar)
		{
			stream = client.GetStream();
			var bytes = Encoding.ASCII.GetBytes("get ayende\r\n");
			stream.BeginWrite(bytes, 0, bytes.Length, OnFinishedWritingGet, null);
		}

		private void OnFinishedWritingGet(IAsyncResult ar)
		{
			stream.EndWrite(ar);
			stream.BeginRead(buffer, 0, buffer.Length, OnFinishedReadingGet, null);
		}

		private void OnFinishedReadingGet(IAsyncResult ar)
		{
			stream.EndRead(ar);

			var val = increment();
			if (val > 10000)
			{
				wait.Set();
				return;
			}

			var bytes = Encoding.ASCII.GetBytes("get ayende" + val + "\r\n");
			stream.BeginWrite(bytes, 0, bytes.Length, OnFinishedWritingGet, null);
		}
	}

	public class WriteData
	{
		readonly byte[] buffer = new byte[2048];
		private Stream stream;
		private readonly Func<int> increment;
		private readonly TcpClient client;
		private readonly ManualResetEvent wait;

		public WriteData(TcpClient client, Func<int> increment, ManualResetEvent wait)
		{
			this.client = client;
			this.wait = wait;
			this.increment = increment;
		}

		public void BeginWriting(IAsyncResult ar)
		{
			stream = client.GetStream();
			var bytes = Encoding.ASCII.GetBytes("set ayende 0 60 3\r\nabc\r\n");
			stream.BeginWrite(bytes, 0, bytes.Length, OnFinishedWritingSet, null);
		}

		private void OnFinishedWritingSet(IAsyncResult ar)
		{
			stream.EndWrite(ar);
			stream.BeginRead(buffer, 0, buffer.Length, OnFinishedReadingSet, null);
		}

		private void OnFinishedReadingSet(IAsyncResult ar)
		{
			stream.EndRead(ar);

			var val = increment();
			if (val > 10000)
			{
				wait.Set();
				return;
			}

			var bytes = Encoding.ASCII.GetBytes("set ayende"+val+" 0 60 3\r\nabc\r\n");
			stream.BeginWrite(bytes, 0, bytes.Length, OnFinishedWritingSet, null);
		}
	}
}