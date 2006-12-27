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
    		return new DateTime(date.Year, date.Month,1);
    	}
    	
    	public static DateTime EndMonth(DateTime date)
    	{
			return StartMonth(date).AddMonths(1).AddDays(-1);
    	}
        
        public static DateTime Next(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek <= DateTime.Today.DayOfWeek)
            {
                return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek) + 7);
            }

            return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek)); 
        }

        public static DateTime Previous(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek >= DateTime.Today.DayOfWeek)
            {
                return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek) - 7);
            }

            return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek));
        }

        public static DateTime StartWeek(DateTime time)
        {
            return time.Date.AddDays(DayOfWeek.Sunday - time.DayOfWeek);
        }
        
        public static DateTime EndWeek(DateTime time)
        {
            return time.Date.AddDays(DayOfWeek.Saturday - time.DayOfWeek);
        }
    }
}
