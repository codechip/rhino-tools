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
    }
}
