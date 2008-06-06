using System.Collections;
using MbUnit.Framework;
using NMemcached.Util;

namespace NMemcached.Tests
{
	[TestFixture]
	public class SystemWebCacheTests : CacheMixin
	{
		[SetUp]
		public void Setup()
		{
			ClearCache();
		}

		[Test]
		public void IterationWithModifications()
		{
			Cache["foo"] = new object();
			Cache["bar"] = new object();

			foreach (DictionaryEntry de in Cache)
			{
				Cache.Remove(de.Key.ToString());
			}
			Assert.AreEqual(0, Cache.Count);
		}

		[Test]
		public void IterationWithAdditions_ShouldGet_ConsistentSnapshot()
		{
			Cache["foo"] = new object();
			Cache["bar"] = new object();
			bool first = true;
			int count = 0;
			foreach (DictionaryEntry de in Cache)
			{
				count += 1;
				if(first==false)
					Cache["baz"] = new object();
				first = false;
			}
			Assert.AreEqual(2, count);
			Assert.AreEqual(3, Cache.Count);
		}

		[Test]
		public void IterationWithRemoval_ShouldGet_ConsistentSnapshot()
		{
			Cache["foo"] = new object();
			Cache["bar"] = new object();
			bool first = true;
			int count = 0;
			foreach (DictionaryEntry de in Cache)
			{
				count += 1;
				if (first)
					Cache.Remove("bar");
				Assert.IsNotNull(de.Value);
				Assert.IsNotNull(de.Key);
				first = false;
			}
			Assert.AreEqual(2, count);
			Assert.AreEqual(1, Cache.Count);
		}
	}
}