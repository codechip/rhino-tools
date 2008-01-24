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

namespace Rhino.Commons
{
	using System;

	/// <summary>
	/// Helper class for dates
	/// </summary>
    public static class DateUtil
    {
		/// <summary>
		/// Combines the date part of the first date with the 
		/// time part of the second part
		/// </summary>
		/// <param name="datePart">The date part.</param>
		/// <param name="timePart">The time part.</param>
		/// <returns></returns>
        public static DateTime Combine(DateTime datePart, DateTime timePart)
        {
            return new DateTime(datePart.Year, datePart.Month, datePart.Day, 
                timePart.Hour,  timePart.Minute, timePart.Second, timePart.Millisecond);
        }

		/// <summary>
		/// Get the start of the month of this date
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
    	public static DateTime StartMonth(DateTime date)
    	{
    		return new DateTime(date.Year, date.Month,1);
    	}

		/// <summary>
		/// Get the end of the month from this date
		/// </summary>
		/// <param name="date">The date.</param>
		/// <returns></returns>
    	public static DateTime EndMonth(DateTime date)
    	{
			return StartMonth(date).AddMonths(1).AddDays(-1);
    	}

		/// <summary>
		/// Get the next date that match the day of the week
		/// </summary>
		/// <param name="dayOfWeek">The day of week.</param>
		/// <returns></returns>
        public static DateTime Next(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek <= DateTime.Today.DayOfWeek)
            {
                return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek) + 7);
            }

            return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek)); 
        }

		/// <summary>
		/// Get the previous date that match the day of the week
		/// </summary>
		/// <param name="dayOfWeek">The day of week.</param>
		/// <returns></returns>
        public static DateTime Previous(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek >= DateTime.Today.DayOfWeek)
            {
                return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek) - 7);
            }

            return DateTime.Today.Date.AddDays((dayOfWeek - DateTime.Today.DayOfWeek));
        }

		/// <summary>
		/// Get the start of the ween this date is on
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns></returns>
        public static DateTime StartWeek(DateTime time)
        {
            return time.Date.AddDays(DayOfWeek.Sunday - time.DayOfWeek);
        }

		/// <summary>
		/// Get the end of the ween this date is on
		/// </summary>
		/// <param name="time">The time.</param>
		/// <returns></returns>
        public static DateTime EndWeek(DateTime time)
        {
            return time.Date.AddDays(DayOfWeek.Saturday - time.DayOfWeek);
        }
    }
}
