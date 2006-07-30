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
    }
}
