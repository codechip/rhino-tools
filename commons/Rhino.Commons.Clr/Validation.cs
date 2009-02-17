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
	/// Helper class for performing validation
	/// </summary>
	[Obsolete("This class is scheduled to be removed, use the Guard class instead.")]
    public static class Validation
    {
		/// <summary>
		/// Validate that the start date comes beore the end.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
        public static void DateRange(DateTime start, DateTime end)
        {
            if(start > end)
            {
                throw new ArgumentException("The start date cannot come after the end date");
            }
        }

		/// <summary>
		/// Validate that the string in not null or empty
		/// </summary>
		/// <param name="str">The STR.</param>
		/// <param name="name">The name.</param>
        public static void NotNullOrEmpty(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("{0} must have a value", name);
        }


		/// <summary>
		/// Validate that the object is not null
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="paramName">Name of the param.</param>
        public static void NotNull(object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

		/// <summary>
		/// Validate overlap of comparables
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="obj">The obj.</param>
		/// <param name="paramName">Name of the param.</param>
        public static void InRange(IComparable start, IComparable end, IComparable obj, string paramName)
        {
            if (start.CompareTo(obj) > 0 || end.CompareTo(obj) < 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

		/// <summary>
		/// Validate overlap of dates
		/// </summary>
		/// <param name="innerStart">The inner start.</param>
		/// <param name="innerEnd">The inner end.</param>
		/// <param name="outerStart">The outer start.</param>
		/// <param name="outerEnd">The outer end.</param>
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

		/// <summary>
		/// Validate that the number is positive
		/// </summary>
		/// <param name="number">The number.</param>
		/// <param name="name">The name.</param>
        public static void PositiveNumber(int number, string name)
        {
            if (number < 0)
                throw new ArgumentException(
                    string.Format("{1} should be positive, but was {0}", number, name));
        }
    }
}
