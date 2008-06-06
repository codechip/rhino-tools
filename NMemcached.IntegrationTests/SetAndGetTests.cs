using System;
using System.Text;
using System.Threading;
using MbUnit.Framework;
using NMemcached.Model;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class SetAndGetTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Can_set_item_in_server()
		{
			bool set = client.Set("ayende", "test");
			Assert.IsTrue(set);
			Assert.AreEqual("test", Encoding.ASCII.GetString(((CachedItem)Cache["ayende"]).Buffer));
		}

		[Test]
		public void Can_set_item_in_server_with_timespan_expiry()
		{
			bool set = client.Set("ayende", "test", TimeSpan.FromSeconds(1));
			Assert.IsTrue(set);
			Thread.Sleep(1200);
			Assert.IsNull(client.Get("ayende"));
		}

		[Test]
		public void Can_set_item_in_server_with_datetime_expiry()
		{
			bool set = client.Set("ayende", "test", DateTime.Now.AddMilliseconds(10));
			Assert.IsTrue(set);
			Thread.Sleep(100);
			Assert.IsNull(client.Get("ayende"));
		}

		[Test]
		public void Can_set_several_items_in_server()
		{
			bool set = client.Set("ayende", "test");
			Assert.IsTrue(set);
			set = client.Set("rahien", "test");
			Assert.IsTrue(set);
			set = client.Set("blar", "hlar");
			Assert.IsTrue(set);
			set = client.Set("foo", "boo");
			Assert.IsTrue(set);
		}

		[Test]
		public void Can_get_several_items_at_once()
		{
			bool set = client.Set("ayende", "test");
			Assert.IsTrue(set);
			set = client.Set("rahien", "test");
			Assert.IsTrue(set);
			set = client.Set("blar", "hlar");
			Assert.IsTrue(set);
			set = client.Set("foo", "boo");
			Assert.IsTrue(set);

			object[] objects = client.Get(new []{"ayende","rahien","blar","foo"});
			Assert.AreEqual("test", objects[0]);
			Assert.AreEqual("test", objects[1]);
			Assert.AreEqual("hlar", objects[2]);
			Assert.AreEqual("boo", objects[3]);
		}

		[Test]
		public void Can_set_and_item_in_server()
		{
			client.Set("ayende", "test");
			string cached = (string)client.Get("ayende");
			Assert.AreEqual("test", cached);
		}

		[Test]
		public void Can_get_cas_value_from_server()
		{
			client.Set("ayende", "test");
			ulong unique;
			client.Gets("ayende", out unique);
			Assert.IsTrue(0 != unique);
			
		}
	}
}