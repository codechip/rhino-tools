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
    [TestFixture]
    public class DateRangeTests
    {
        public static DateTime start = new DateTime(2005, 10, 8);
        public static DateTime end = new DateTime(2005, 12, 20);

        [Test]
        public void Ctor()
        {
            DateRange range = new DateRange(start, end);

            Assert.AreEqual(start, range.Start);
            Assert.AreEqual(end, range.End);

            TimeSpan span = end.Subtract(start);

            Assert.AreEqual(span, range.Span);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), "The start date cannot come after the end date")]
        public void EndBeforeStart()
        {
			new DateRange(end, start);
        }

        [Test]
        public void ForEachDay()
        {
            int count = 0;
            int dayCount = 73;
            DateTime[] dates = new DateTime[dayCount];
            DateTime tmp = start;
            for (int i = 0; i < dayCount; i++)
            {
                dates[i] = tmp;
                tmp = tmp.AddDays(1);
            }
            TimeSpan span = end.Subtract(start);

            DateRange range = new DateRange(start, end);
            range.ForEachDay(delegate(DateTime date)
                                 {
                                     if (count!=dates.Length)//skip last day
                                        Assert.AreEqual(dates[count], date);                                     
                                     count++;
                                 });
            Assert.AreEqual(dayCount+1, count);
        }

        [Test]
        public void ForEachWeek()
        {
            int count = 0;
            int weekCount = 12;
            DateRange[] weeks = new DateRange[weekCount];
            DateRange range = new DateRange(start, end);
            weeks[0] = new DateRange(start, start);
            DateTime current = start.AddDays(1);
            for (int i = 1; i < weekCount-1; i++)
            {
                weeks[i] = new DateRange(current, current.AddDays(6));
                current = current.AddDays(7);
            }
            weeks[weekCount - 1] = new DateRange(current, end);
            range.ForEachWeek(delegate(DateRange week)
                                 {
                                      Assert.AreEqual(weeks[count], week);
                                     count++;
                                 });
            Assert.AreEqual(weekCount, count);
        }

        [Test]
        public void ForEachWeek_Full()
        {
            int count = 0;
            int weekCount = 2;
            DateRange[] weeks = new DateRange[weekCount];
            DateTime weekEnd = new DateTime(2005, 12, 17);
            DateTime weekStart = new DateTime(2005, 12, 4);
            DateRange range = new DateRange(weekStart, weekEnd);
            weeks[0] = new DateRange(weekStart,new DateTime(2005, 12, 10));
            weeks[1] = new DateRange(new DateTime(2005, 12,11), weekEnd);
            range.ForEachWeek(delegate(DateRange week)
                                 {
                                     Assert.AreEqual(weeks[count], week);
                                     count++;
                                 });
            Assert.AreEqual(weekCount, count);
        }
        
        [Test]
        public void ForEachWeek_Partial()
        {
            int count = 0;
            int weekCount = 1;
            DateRange[] weeks = new DateRange[weekCount];
            DateRange range = new DateRange(start, start);
            weeks[0] = new DateRange(start, start);
            range.ForEachWeek(delegate(DateRange week)
                                 {
                                     Assert.AreEqual(weeks[count], week);
                                     count++;
                                 });
            Assert.AreEqual(weekCount, count);
        }
        
        [Test]
        public void ForEachMonth()
        {
            int count = 0;
            int monthsCount = 3;
            DateRange[] months = new DateRange[monthsCount];
            months[0] = new DateRange(start, new DateTime(2005, 10, 31));
            months[1] = new DateRange(new DateTime(2005, 11, 1), 
                                      new DateTime(2005, 11, 30));
            months[2] = new DateRange(new DateTime(2005, 12, 1), end);
            DateRange range = new DateRange(start, end);
            range.ForEachMonth(delegate(DateRange month)
                                 {
                                     Assert.AreEqual(months[count], month);
                                     count++;
                                 });
            Assert.AreEqual(monthsCount, count);
        }

        [Test]
        public void ForEachMonth_Full()
        {
            int count = 0;
            TimeSpan span = end.Subtract(start);

            DateRange range = new DateRange(new DateTime(2005, 10, 1), new DateTime(2005, 10, 31));
            range.ForEachMonth(delegate(DateRange month)
                                 {
                                     Assert.AreEqual(range, month);  
                                     count++;
                                 });
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ForEachMonth_SameDay()
        {
            int count = 0;
            TimeSpan span = end.Subtract(start);

            DateRange range = new DateRange(new DateTime(2005, 10, 1), new DateTime(2005, 10, 1));
            range.ForEachMonth(delegate(DateRange month)
                                 {
                                     Assert.AreEqual(range, month);
                                     count++;
                                 });
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ForEachMonth_TwoPartialMonths()
        {
            int count = 0;
            int monthsCount = 2;
            DateRange[] months = new DateRange[monthsCount];
            DateTime start = new DateTime(2005, 10, 31);
            months[0] = new DateRange(start, start);
            DateTime end = new DateTime(2005, 11, 1);
            months[1] = new DateRange(end,
                                      end);
            DateRange range = new DateRange(start, end);
            range.ForEachMonth(delegate(DateRange month)
                                 {
                                     Assert.AreEqual(months[count], month);
                                     count++;
                                 });
            Assert.AreEqual(monthsCount, count);
        }   
    }
}
