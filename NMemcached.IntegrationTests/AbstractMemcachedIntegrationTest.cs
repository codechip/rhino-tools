using System.Net;
using BeIT.MemCached;
using MbUnit.Framework;
using NMemcached.Util;

namespace NMemcached.IntegrationTests
{
	public class AbstractMemcachedIntegrationTest : CacheMixin
	{
		protected MemcachedClient client;
		private MemcachedServer server;

		[SetUp]
		public void Setup()
		{
			ClearCache();
			server = new MemcachedServer(IPAddress.Any, 33433);

			if (MemcachedClient.Exists("default") == false)
				MemcachedClient.Setup("default", new[] { "127.0.0.1:33433" });
			
			client = MemcachedClient.GetInstance("default");
			client.SendReceieveTimeout = 250000;
			server.Start();
		}

		[TearDown]
		public void TearDown()
		{
			server.Stop();
		}
	}
}