#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Commons.Test
{
#pragma warning disable 612,618 // obsolete warning
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
#pragma warning restore 612,618

}
