using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rhino.Commons
{
  [DebuggerDisplay("{Start}:{End}")]
  public struct DateRange
  {
    private readonly DateTime start;
    private readonly DateTime end;

    public DateTime Start
    {
      get { return start; }
    }

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

    public TimeSpan Span
    {
      get { return end.Subtract(start); }
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

    //Equals method is provides by ValueType, which does bitwise check, which
    //is what we need.

    public override int GetHashCode()
    {
      return start.GetHashCode() + end.GetHashCode();
    }

    public override string ToString()
    {
      return start.ToString() + " - " + end.ToString();
    }
  }
}
