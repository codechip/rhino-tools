using System;
using System.Net;
using System.Threading;

namespace NMemcached
{
	public class Program
	{
		static void Main()
		{
			var server = new MemcachedServer(IPAddress.Any, 11211);
			server.Start();
			Thread.CurrentThread.Join();
		}
	}
}
