using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace BookStore.Tests
{
    [TestFixture]
    public class BasicTest
    {
        [Test]
        public void DoesItWork()
        {
            Assert.AreEqual(5, 4 + 1);
        }
    }
}
