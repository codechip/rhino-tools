using MbUnit.Framework;

namespace NMemcached.IntegrationTests
{
	[TestFixture]
	public class AppendAndPrependTests : AbstractMemcachedIntegrationTest
	{
		[Test]
		public void Will_return_error_if_trying_to_append_to_non_existing_item()
		{
			bool append = client.Append("ayende", "fo");
			Assert.IsFalse(append);
		}

		[Test]
		public void Can_append_items_on_the_server()
		{
			client.Set("ayende", "this is a ");
			bool append = client.Append("ayende", "test");
			Assert.IsTrue(append);
		}

		[Test]
		public void Can_get_items_appended_on_the_server()
		{
			client.Set("ayende", "this is a ");
			client.Append("ayende", "test");

			Assert.AreEqual("this is a test", client.Get("ayende"));
		}

		[Test]
		public void Will_return_error_if_trying_to_prepend_to_non_existing_item()
		{
			bool append = client.Prepend("ayende", "fo");
			Assert.IsFalse(append);
		}

		[Test]
		public void Can_prepend_items_on_the_server()
		{
			client.Set("ayende", "this is a ");
			bool append = client.Prepend("ayende", "test");
			Assert.IsTrue(append);
		}

		[Test]
		public void Can_get_items_prepended_on_the_server()
		{
			client.Set("ayende", "this is");
			client.Prepend("ayende", "test, ");
			//yuda
			Assert.AreEqual("test, this is", client.Get("ayende"));
		}
	}
}