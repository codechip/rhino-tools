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

		[Property(NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		public DateTime Start
		{
			get { return start; }
		}

		[Property(NotNull = true, Access = PropertyAccess.FieldCamelcase)]
		public DateTime End
		{
			get { return end; }
		}

		public DateRange(DateTime start, DateTime end)
		{
			Validation.DateRange(start, end);
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
			return Overlap(range.start) || Overlap(range.end);
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
