namespace Rhino.Queues.Tests.Units
{
	using System;
	using Impl;
	using MbUnit.Framework;

	[TestFixture]
	public class SizedLimitedSetTests
	{
		[Test]
		[ExpectedArgumentException("maxCount cannot be less than 1")]
		public void Cannot_create_will_size_of_one()
		{
			new SizedLimitedSet<Guid>(1);
		}

		[Test]
		[ExpectedArgumentException("maxCount cannot be less than 1")]
		public void Cannot_create_will_size_of_less_than_one()
		{
			new SizedLimitedSet<Guid>(-1);
		}

		[Test]
		public void Can_create_with_size_larger_than_one()
		{
			new SizedLimitedSet<Guid>(2);
		}

		[Test]
		public void Will_be_limited_to_specified_size()
		{
			var set = new SizedLimitedSet<Guid>(2);
			set.Add(Guid.NewGuid());
			set.Add(Guid.NewGuid());

			for (int i = 0; i < 50; i++)
			{
				set.Add(Guid.NewGuid());
				Assert.AreEqual(2, set.Count);
			}
		}

		[Test]
		public void Will_remove_old_items_from_set()
		{
			var set = new SizedLimitedSet<Guid>(2);
			var guid1 = Guid.NewGuid();
			var guid2 = Guid.NewGuid();
			set.Add(guid1);
			set.Add(guid2);

			for (int i = 0; i < 50; i++)
			{
				Assert.IsTrue(set.Exists(guid1)); 
				Assert.IsTrue(set.Exists(guid2));
				guid1 = guid2;
				guid2 = Guid.NewGuid();
				set.Add(guid2);
				Assert.AreEqual(2, set.Count);
			}
		}
	}
}