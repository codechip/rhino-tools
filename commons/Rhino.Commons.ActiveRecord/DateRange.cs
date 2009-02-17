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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Castle.ActiveRecord;

namespace Rhino.Commons
{
	[DebuggerDisplay("{Start}:{End}")]
	public struct DateRange
	{
		private DateTime start;
		private DateTime end;

		[Property(Column = "StartDate", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		public DateTime Start
		{
			get { return start; }
		}

		[Property(Column = "EndDate", NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		public DateTime End
		{
			get { return end; }
		}

		public DateRange(DateTime start, DateTime end)
		{
			Guard.Against<ArgumentException>(start > end, "The start date cannot come after the end date");
			this.start = start;
			this.end = end;
		}

		public bool IsEmpty
		{
			get { return (Span.Ticks == 0); }
		}

		public TimeSpan Span
		{
			get { return end.Subtract(start); }
		}

		public bool Overlap(DateTime date)
		{
			return start <= date && date <= end;
		}

		public bool Overlap(TimeSpan time)
		{
			TimeSpan start_TimeOfDay = start.TimeOfDay;
			TimeSpan end_TimeOfDay = end.TimeOfDay;
			if (start_TimeOfDay > end_TimeOfDay)
			{
				end_TimeOfDay = end_TimeOfDay.Add(TimeSpan.FromDays(1));
			}
			return start_TimeOfDay <= time && time <= end_TimeOfDay;
		}

		public bool Overlap(DateRange range)
		{
		    return !(range.start >= this.end | range.end <= this.start);
		}

		public void ForEachDay(Proc<DateTime> action)
		{
			foreach (DateTime day in Days)
			{
				action(day);
			}
		}

		public IEnumerable<DateTime> Days
		{
			get
			{
				DateTime current = start.Date;
				DateTime endDate = end.Date;
				//Changed by bernie 
				//while (current != endDate)
				while (current <= endDate)
				{
					yield return current;
					current = current.AddDays(1);
				}
			}
		}

		public void ForEachWeek(Proc<DateRange> action)
		{
			foreach (DateRange week in Weeks)
			{
				action(week);
			}
		}

		private IEnumerable<DateRange> Weeks
		{
			get
			{
				DateTime current = start;
				DateTime weekStart;
				while (current.AddDays(1).DayOfWeek != DayOfWeek.Sunday
					   && current < end)
				{
					current = current.AddDays(1);
				}
				yield return new DateRange(start, current);
				weekStart = current.AddDays(1);//current is the last day of the week
				while (weekStart < end && weekStart.AddDays(7) < end)
				{
					yield return new DateRange(weekStart, weekStart.AddDays(6));
					weekStart = weekStart.AddDays(7);
				}
				if (weekStart <= end)
					yield return new DateRange(weekStart, end);
			}
		}

		public void ForEachMonth(Proc<DateRange> action)
		{
			foreach (DateRange month in Months)
			{
				action(month);
			}
		}

		private IEnumerable<DateRange> Months
		{
			get
			{
				DateTime current = start;
				DateTime monthStart;
				while (current.AddDays(1).Day != 1 && current < end)
				{
					current = current.AddDays(1);
				}
				yield return new DateRange(start, current);
				monthStart = current.AddDays(1);//current is the last day of the month
				while (monthStart < end && monthStart.AddMonths(1) < end)
				{
					yield return new DateRange(monthStart, monthStart.AddMonths(1).AddDays(-1));
					monthStart = monthStart.AddMonths(1);
				}
				if (monthStart <= end)
					yield return new DateRange(monthStart, end);
			}
		}

		public override bool Equals(object obj)
		{
			//Equals method is provides by ValueType, which does bitwise check, which
			//is what we need.
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return start.GetHashCode() + end.GetHashCode();
		}

		public override string ToString()
		{
			return start.ToString() + " - " + end.ToString();
		}

		public string ToString(string format)
		{
			return start.ToString(format) + " - " + end.ToString(format);
		}

		public static DateRange Intersection(DateRange x, DateRange y)
		{
			return new DateRange(
			x.Start > y.Start ? x.Start : y.Start,
			x.End < y.End ? x.End : y.End
			);
		}
	}
}
