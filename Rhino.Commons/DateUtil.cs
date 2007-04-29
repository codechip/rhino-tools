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
