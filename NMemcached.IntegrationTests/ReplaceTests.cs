using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class ReplaceTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Replacing_item_not_on_server_will_fail()
		{
			Assert.IsFalse(client.Replace("foo", 15));
		}

		[Test]
		public void Replacing_item_on_the_server_will_pass()
		{
			client.Add("foo", "bar");
			Assert.IsTrue(client.Replace("foo", 15));
			Assert.AreEqual(15, client.Get("foo"));
		}
	}
}