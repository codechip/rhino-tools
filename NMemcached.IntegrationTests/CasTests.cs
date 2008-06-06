using System;
using BeIT.MemCached;
using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class CasTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Can_get_cas_value_from_server()
		{
			client.Set("ayende", "test");
			ulong unique;
			client.Gets("ayende", out unique);
			Assert.IsTrue(0L != unique);
		}


		[Test]
		public void Can_ask_server_to_modify_using_cas_value()
		{
			client.Set("ayende", "test");
			ulong unique;
			client.Gets("ayende", out unique);
			MemcachedClient.CasResult set = client.CheckAndSet("ayende", "blah", unique);
			Assert.AreEqual(MemcachedClient.CasResult.Stored, set);
			Assert.AreEqual("blah", client.Get("ayende"));
		}

		[Test]
		public void Will_not_update_value_if_does_not_match_cas_value()
		{
			client.Set("ayende", "test");
			ulong unique;
			client.Gets("ayende", out unique);
			MemcachedClient.CasResult set = client.CheckAndSet("ayende", "blah", unique + 2);
			Assert.AreEqual(MemcachedClient.CasResult.Exists, set);
			Assert.AreEqual("test", client.Get("ayende"));
		}

		[Test]
		public void Will_return_not_found_if_item_does_not_exists()
		{
			MemcachedClient.CasResult set = client.CheckAndSet("ayende", "blah", 12L);
			Assert.AreEqual(MemcachedClient.CasResult.NotFound, set);
			Assert.IsNull(client.Get("ayende"));
		}

		[Test]
		public void When_trying_to_cas_a_deleted_value_will_return_not_stored()
		{
			client.Set("ayende", "test");
			ulong unique;
			client.Gets("ayende", out unique);
			client.Delete("ayende", TimeSpan.FromSeconds(50));
			MemcachedClient.CasResult set = client.CheckAndSet("ayende", "blah", unique);
			Assert.AreEqual(MemcachedClient.CasResult.NotStored, set);
			Assert.IsNull(client.Get("ayende"));
		}
	}
}
