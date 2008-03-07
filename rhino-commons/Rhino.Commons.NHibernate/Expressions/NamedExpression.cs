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
using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace Query
{
	public class NamedExpression
	{
		string name;

		public NamedExpression(string name)
		{
			this.name = name;
		}

		public AbstractCriterion Eq(object value)
		{
			return Expression.Eq(name, value);
		}

		public AbstractCriterion Between(object lo, object hi)
		{
			return Expression.Between(name, lo, hi);
		}

		public AbstractCriterion EqProperty(string otherPropertyName)
		{
			return Expression.EqProperty(name, otherPropertyName);
		}


		public SimpleExpression Ge(object value)
		{
			return Expression.Ge(name, value);
		}

		public SimpleExpression Gt(object value)
		{
			return Expression.Gt(name, value);
		}

		public AbstractCriterion In(ICollection values)
		{
			return Expression.In(name, ToArray(values));
		}

		public AbstractCriterion In(object[] values)
		{
			return Expression.In(name, values);
		}

		public AbstractCriterion In<T>(ICollection<T> values)
		{
			object[] arr = ToArrayGeneric(values);
			return Expression.In(name, arr);
		}
		
		private static object[] ToArray(ICollection values)
		{
			object[] arr = new object[values.Count];
			values.CopyTo(arr, 0);
			return arr;
		}
		
		private static object[] ToArrayGeneric<T>(ICollection<T> values)
		{
			T[] arr = new T[values.Count];
			values.CopyTo(arr, 0);
			return ToArray(arr);
		}

		public AbstractCriterion InsensitiveLike(object value)
		{
			return new InsensitiveLikeExpression(name, value);
		}

		public AbstractCriterion InsensitiveLike(string value, MatchMode matchMode)
		{
			return new InsensitiveLikeExpression( name, value, matchMode);
		}

		public AbstractCriterion IsNotNull()
		{
			return new NotNullExpression(name);
		}

		public AbstractCriterion IsNull()
		{
			return new NullExpression(name);
		}

		public SimpleExpression Le(object value)
		{
			return Expression.Le(name, value);
		}

		public AbstractCriterion LeProperty(string otherPropertyName)
		{
			return Expression.LeProperty(name, otherPropertyName);
		}

		public SimpleExpression Like(object value)
		{
			return Expression.Like(name, value);
		}

		public SimpleExpression Like(string value, MatchMode matchMode)
		{
			return Expression.Like(name, value, matchMode);
		}

		public SimpleExpression Lt(object value)
		{
			return Expression.Lt(name, value);
		}

		public AbstractCriterion LtProperty(string otherPropertyName)
		{
			return Expression.LtProperty(name, otherPropertyName);
		}
	}
}