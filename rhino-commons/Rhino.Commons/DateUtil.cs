using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public static class DateUtil
    {
        public static DateTime Combine(DateTime datePart, DateTime timePart)
        {
            return new DateTime(datePart.Year, datePart.Month, datePart.Day, 
                timePart.Hour,  timePart.Minute, timePart.Second, timePart.Millisecond);
        }

		public static DateTime StartMonth(DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static DateTime EndMonth(DateTime date)
		{
			return StartMonth(date).AddMonths(1).AddDays(-1);
		}
    }
}
