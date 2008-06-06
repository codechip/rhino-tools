using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NMemcached.Commands;
using NMemcached.Extensions;

namespace NMemcached
{
	public class MemcachedServer
	{
		private readonly TcpListener listener;
		private int concurrentConnection = 0;

		public int ConcurrentConnection
		{
			get { return concurrentConnection; }
		}

		public MemcachedServer(IPAddress address, int port)
		{
			listener = new TcpListener(address, port);
		}

		public void Start()
		{
			listener.Start();
			listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
		}

		private void BeginAcceptTcpClientCallback(IAsyncResult ar)
		{
			try
			{
				var client = listener.EndAcceptTcpClient(ar);
				Interlocked.Increment(ref concurrentConnection);
				// accept additional connections
				listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);

				var clientConnection = new ClientConnection(client, client.GetStream(), ()=>Interlocked.Decrement(ref concurrentConnection));
				clientConnection.ProcessNextCommand();
			}
			catch (ObjectDisposedException)
			{
			}
		}


		public void Stop()
		{
			listener.Stop();
		}
	}
}