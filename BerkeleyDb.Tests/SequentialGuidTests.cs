using System;
using System.Collections.Generic;
using MbUnit.Framework;
using System.Linq;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class SequentialGuidTests
	{
		[Test]
		public void Will_create_guids_in_sequence_order()
		{
			var guids = new List<Guid>();
			for (int i = 0; i < 100; i++)
			{
				guids.Add(SequentialGuid.Next());
			}

			Guid prev = Guid.Empty;
			for (int i = 0; i < 100; i++)
			{
				Assert.AreEqual(-1, prev.CompareTo(guids[i]));
				prev = guids[i];
			}
		}
	}
}