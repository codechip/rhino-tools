
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace NMemcachedd.PerformanceTests
{
	class Program
	{
		static int readCycles = 0;
		static int writeCycles = 0;
		private static readonly ManualResetEvent read = new ManualResetEvent(false);
		private static readonly ManualResetEvent write = new ManualResetEvent(false);
		static void Main()
		{
			var clients = new List<TcpClient>();
			var count = 20;
			for (int i = 0; i < count; i++)
			{
				clients.Add(new TcpClient());	
			}
			Console.WriteLine("created clients, starting to connect");
			var startNew = Stopwatch.StartNew();
			for (int i = 0; i < count; i++)
			{
				var client = clients[i];
				if(i%2==0)
				{
					var data = new ReadData(client, () => Interlocked.Increment(ref readCycles), read);
					client.BeginConnect("127.0.0.1", 11211, data.BeginReading, client);
				}
				else
				{
					var data = new WriteData(client, () => Interlocked.Increment(ref writeCycles), write);
					client.BeginConnect("127.0.0.1", 11211, data.BeginWriting, client);
				}
			}
			WaitHandle.WaitAll(new WaitHandle[] { read, write });

			startNew.Stop();
			Console.WriteLine("took " + startNew.ElapsedMilliseconds + " total " + readCycles + " reads and " + writeCycles + " writes using " + count + " connections");
		}
	}
}
