using System;
using System.Threading;
using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class FlushAllTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Can_remove_all_items_from_cache()
		{
			for (int i = 0; i < 50; i++)
			{
				client.Set("i-" + i, i);
			}
			client.FlushAll();

			for (int j = 0; j < 50; j++)
			{
				Assert.IsNull(client.Get("i-"+j));
			}
		}

		[Test]
		public void Can_remove_all_items_from_cache_with_delay()
		{
			for (int i = 0; i < 50; i++)
			{
				client.Set("i-" + i, i);
			}
			client.FlushAll(TimeSpan.FromSeconds(1));
			for (int j = 0; j < 50; j++)
			{
				Assert.AreEqual(j, client.Get("i-" + j));
			}
			Thread.Sleep(1100);
			for (int y = 0; y < 50; y++)
			{
				Assert.IsNull(client.Get("i-" + y));
			}
		}
	}
}