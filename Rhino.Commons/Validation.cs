using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public static class Validation
    {
        public static void DateRange(DateTime start, DateTime end)
        {
            if(start > end)
            {
                throw new ArgumentException("The start date cannot come after the end date");
            }
        }

        public static void NotNullOrEmpty(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("{0} must have a value", name);
        }

        
        public static void NotNull(object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }
        
        public static void InRange(IComparable start, IComparable end, IComparable obj, string paramName)
        {
            if (start.CompareTo(obj) > 0 || end.CompareTo(obj) < 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        public static void InDateRange(
         DateTime innerStart, DateTime innerEnd,
         DateTime outerStart, DateTime outerEnd)
        {
            if (innerStart < outerStart || innerStart > outerEnd ||
                innerEnd > outerEnd || innerEnd < outerStart)
                throw
                    new ArgumentOutOfRangeException(
                        string.Format("Date Ranges do not overlap, {0}-{1} does not contain {2}-{3}",
                                      outerStart.ToShortDateString(), outerEnd.ToShortDateString(),
                                      innerStart.ToShortDateString(), innerEnd.ToShortDateString()));
        }

        public static void PositiveNumber(int number, string name)
        {
            if (number < 0)
                throw new ArgumentException(
                    string.Format("{1} should be positive, but was {0}", number, name));
        }
    }
}
