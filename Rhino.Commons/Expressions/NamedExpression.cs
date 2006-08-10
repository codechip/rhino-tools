using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Expression;

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
			return new BetweenExpression(name, lo, hi);
		}

		public AbstractCriterion EqProperty(string otherPropertyName)
		{
			return new EqPropertyExpression(name, otherPropertyName);
		}


		public SimpleExpression Ge(object value)
		{
			return new GeExpression(name, value);
		}

		public SimpleExpression Gt(object value)
		{
			return new GtExpression(name, value);
		}

		public AbstractCriterion In(ICollection values)
		{
			return new InExpression(name, ToArray(values));
		}

		public AbstractCriterion In(object[] values)
		{
			return new InExpression(name, values);
		}

		public AbstractCriterion In<T>(ICollection<T> values)
		{
			object[] arr = ToArrayGeneric(values);
			return new InExpression(name, arr);
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
			return new InsensitiveLikeExpression(name, value, matchMode);
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
			return new LeExpression(name, value);
		}

		public AbstractCriterion LeProperty(string otherPropertyName)
		{
			return new LePropertyExpression(name, otherPropertyName);
		}

		public SimpleExpression Like(object value)
		{
			return new LikeExpression(name, value);
		}

		public SimpleExpression Like(string value, MatchMode matchMode)
		{
			return new LikeExpression(name, value, matchMode);
		}

		public SimpleExpression Lt(object value)
		{
			return new LtExpression(name, value);
		}

		public AbstractCriterion LtProperty(string otherPropertyName)
		{
			return new LtPropertyExpression(name, otherPropertyName);
		}
	}
}