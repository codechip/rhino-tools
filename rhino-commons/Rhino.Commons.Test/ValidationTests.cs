using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class ValidationTests
    {
        [Test] 
        public void ValidDateRange()
        {
            Validation.DateRange(DateRangeTests.start, DateRangeTests.end);
        }

        [Test]
        public void ValidDateRange_Same()
        {
            Validation.DateRange(DateRangeTests.start, DateRangeTests.start);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), "The start date cannot come after the end date")]
        public void InvalidDateRange()
        {
            Validation.DateRange(DateRangeTests.end, DateRangeTests.start);
        }

        [Test]
        public void NotNull_NotNull()
        {
            Validation.NotNull(new object(), "");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException), "Value cannot be null.\r\nParameter name: foo")]
        public void NotNull_Null()
        {
            Validation.NotNull(null, "foo");
        }


        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Specified argument was out of the range of valid values.\r\nParameter name: precentage")]
        public void IsInRange_NotInRange()
        {
            Validation.InRange(1, 100, 233, "precentage");
        }

        [Test]
        public void IsInRange_InRange()
        {
            Validation.InRange(1, 100, 50, "precentage");
        }
        
    }
}
