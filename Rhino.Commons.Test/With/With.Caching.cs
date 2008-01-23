namespace Rhino.Commons.Test
{
	using Commons;
	using MbUnit.Framework;

	[TestFixture]
	public class WithCachingFixture
	{
		[Test]
		public void CanEnterCachingMode()
		{
			Assert.IsFalse(With.Caching.Enabled);
			using (With.QueryCache())
			{
				Assert.IsTrue(With.Caching.Enabled);
			}
			Assert.IsFalse(With.Caching.Enabled);
		}

		[Test]
		public void CanEnterCachingModeRecursively()
		{
			Assert.IsFalse(With.Caching.Enabled);
			using (With.QueryCache())
			{
				Assert.IsTrue(With.Caching.Enabled);
				using (With.QueryCache())
				{
					Assert.IsTrue(With.Caching.Enabled);

					using (With.QueryCache())
					{
						Assert.IsTrue(With.Caching.Enabled);
					}
					Assert.IsTrue(With.Caching.Enabled);
				}
				Assert.IsTrue(With.Caching.Enabled);
			}
			Assert.IsFalse(With.Caching.Enabled);
		}
	}
}