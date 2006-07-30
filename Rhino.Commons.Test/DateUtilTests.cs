using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class DateUtilTests
    {
        [Test]
        public void CombineTest()
        {
            DateTime datePart = new DateTime(2005, 12, 20);
            DateTime timePart = new DateTime(1981, 12, 20, 22, 55, 00);
            
            DateTime expected = new DateTime(2005, 12, 20, 22, 55, 00);

            DateTime actual = DateUtil.Combine(datePart, timePart);
            
            Assert.AreEqual(expected, actual);
        }

    }
}
