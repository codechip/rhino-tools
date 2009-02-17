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
using System.Text;

namespace Rhino.Mocks.Constraints
{

	/// <summary>
	/// Provides access to the constraintes defined in the class <see cref="Is"/> to be used in context
	/// with the <see cref="Arg&lt;T&gt;"/> syntax.
	/// </summary>
	/// <typeparam name="T">The type of the argument</typeparam>
	public class IsArg<T>
	{

		internal IsArg() { }


		/// <summary>
		/// Evaluate a greater than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than</param>
		public T GreaterThan(IComparable objToCompare)
		{
			ArgManager.AddInArgument(Is.GreaterThan(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a less than constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than</param>
		public T LessThan(IComparable objToCompare)
		{
			ArgManager.AddInArgument(Is.LessThan(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a less than or equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be less than or equal to</param>
		public T LessThanOrEqual(IComparable objToCompare)
		{
			ArgManager.AddInArgument(Is.LessThanOrEqual(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate a greater than or equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="objToCompare">The object the parameter should be greater than or equal to</param>
		public T GreaterThanOrEqual(IComparable objToCompare)
		{
			ArgManager.AddInArgument(Is.GreaterThanOrEqual(objToCompare));
			return default(T);
		}

		/// <summary>
		/// Evaluate an equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should equal to</param>
		public T Equal(object obj)
		{
			ArgManager.AddInArgument(Is.Equal(obj));
			return default(T);
		}


		/// <summary>
		/// Evaluate a not equal constraint for <see cref="IComparable"/>.
		/// </summary>
		/// <param name="obj">The object the parameter should not equal to</param>
		public T NotEqual(object obj)
		{
			ArgManager.AddInArgument(Is.NotEqual(obj));
			return default(T);
		}


		/// <summary>
		/// Evaluate a same as constraint.
		/// </summary>
		/// <param name="obj">The object the parameter should the same as.</param>
		public T Same(object obj)
		{
			ArgManager.AddInArgument(Is.Same(obj));
			return default(T);
		}

		/// <summary>
		/// Evaluate a not same as constraint.
		/// </summary>
		/// <param name="obj">The object the parameter should not be the same as.</param>
		public T NotSame(object obj)
		{
			ArgManager.AddInArgument(Is.NotSame(obj));
			return default(T);
		}

		/// <summary>
		/// A constraints that accept anything
		/// </summary>
		/// <returns></returns>
		public T Anything
		{
			get
			{
				ArgManager.AddInArgument(Is.Anything());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accept only nulls
		/// </summary>
		/// <returns></returns>
		public T Null
		{
			get
			{
				ArgManager.AddInArgument(Is.Null());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accept only non null values
		/// </summary>
		/// <returns></returns>
		public T NotNull
		{
			get
			{
				ArgManager.AddInArgument(Is.NotNull());
				return default(T);
			}
		}

		/// <summary>
		/// A constraint that accept only value of the specified type.
		/// The check is performed on the type that has been defined
		/// as the argument type.
		/// </summary>
		public T TypeOf
		{
			get
			{
				ArgManager.AddInArgument(Is.TypeOf<T>());
				return default(T);
			}
		}

		/// <summary>
		/// Throws NotSupportedException. Don't use Equals to define constraints. Use Equal instead.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("Don't use Equals() to define constraints, use Equal() instead");
		}

		/* implement GetHashCode to avoid compiler warning */
		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

}
