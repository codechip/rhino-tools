using System;
using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class AddTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Can_add_item_if_does_not_already_exists()
		{
			bool success = client.Add("ayende", DateTime.Now);
			Assert.IsTrue(success);
		}

		[Test]
		public void Can_not_add_item_if_already_exists()
		{
			bool success = client.Add("ayende", DateTime.Now);
			Assert.IsTrue(success);

			success = client.Add("ayende", DateTime.Now);
			Assert.IsFalse(success);
		}

		[Test]
		public void Calling_set_and_then_add_will_fail_the_add()
		{
			bool success = client.Set("ayende", DateTime.Now);
			Assert.IsTrue(success);

			success = client.Add("ayende", DateTime.Now);
			Assert.IsFalse(success);
		}

		[Test]
		public void Calling_add_and_then_set_will_pass()
		{
			bool success = client.Add("ayende", DateTime.Now);
			Assert.IsTrue(success);

			success = client.Set("ayende", DateTime.Now);
			Assert.IsTrue(success);
		}
	}
}