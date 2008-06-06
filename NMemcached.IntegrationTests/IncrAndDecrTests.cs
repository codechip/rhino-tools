using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class IncrAndDecrTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Trying_to_incr_item_not_in_chace_would_fail()
		{
			ulong? increment = client.Increment("abc", 5);
			Assert.IsNull(increment);
		}

		[Test]
		public void Can_incr_item_in_chace()
		{
			client.Set("abc", "12312");
			ulong? increment = client.Increment("abc", 5);
			Assert.AreEqual(12312+5, increment);
		}

		[Test]
		public void Can_incr_and_overflow_over_UInt64()
		{
			client.Set("abc", ulong.MaxValue);
			ulong? increment = client.Increment("abc", 5);
			Assert.AreEqual(5, increment);
		}

		[Test]
		public void Trying_to_decr_item_not_in_chace_would_fail()
		{
			ulong? increment = client.Decrement("abc", 5);
			Assert.IsNull(increment);
		}

		[Test]
		public void Can_decr_item_in_chace()
		{
			client.Set("abc", "12312");
			ulong? increment = client.Decrement("abc", 5);
			Assert.AreEqual(12312 - 5, increment);
		}


		[Test]
		public void Cannot_underflow_using_decr()
		{
			client.Set("abc", 0);
			ulong? increment = client.Decrement("abc", 5);
			Assert.AreEqual(0, increment);
		}
	}
}