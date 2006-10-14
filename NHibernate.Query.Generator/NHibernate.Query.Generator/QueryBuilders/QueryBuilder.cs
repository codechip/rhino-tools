using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Expression;

namespace Query
{
	public class QueryBuilder<T>
	{
		protected string name;
		/// <summary>
		/// This is needed so we can support
		/// Where.Comment.Post == post
		/// And it will turn to Expression.Eq("Post", post)  on the root rather than on a sub
		/// criteria
		/// </summary>
		protected string assoicationPath;
		private bool backTrackAssoicationsOnEquality;
		private ICollection<QueryBuilder<T>> children = new List<QueryBuilder<T>>();
		private ICollection<ICriterion> criterions = new List<ICriterion>();

		public QueryBuilder(string name, string assoicationPath, bool backTrackAssoicationsOnEquality)
		{
			this.name = name;
			this.assoicationPath = assoicationPath;
			this.backTrackAssoicationsOnEquality = backTrackAssoicationsOnEquality;
		}

		public QueryBuilder(string name, string assoicationPath)
		{
			this.name = name;
			this.assoicationPath = assoicationPath ?? "this";
		}

		protected void AddCriterion(AbstractCriterion criterion)
		{
			criterions.Add(criterion);
		}

		public QueryBuilder<T> Eq(object value)
		{
			AbstractCriterion eq;
			if (value == null)
				eq = Expression.IsNull(name);
			else
				eq = Expression.Eq(name, value);
			QueryBuilder<T> self = this;
			if(backTrackAssoicationsOnEquality)
			{
				self = new QueryBuilder<T>(name, BackTrackAssoicationPath(assoicationPath));
				children.Add(self);
			}
			self.AddCriterion(eq);
			return this;
		}


		public QueryBuilder<T> NotEq(object value)
		{
			AbstractCriterion eq;
			if (value == null)
				eq = Expression.IsNotNull(name);
			else 
				eq = Expression.Not(Expression.Eq(name, value));
			QueryBuilder<T> self = this;
			if (backTrackAssoicationsOnEquality)
			{
				self = new QueryBuilder<T>(name, BackTrackAssoicationPath(assoicationPath));
				children.Add(self);
			}
			self.AddCriterion(eq);
			return this;
		}


		public QueryBuilder<T> In(ICollection values)
		{
			AbstractCriterion inExpression = new InExpression(name, ToArray(values));
			AddCriterion(inExpression);
			return this;
		}

		public QueryBuilder<T> In(params object[] values)
		{
			AbstractCriterion inExpression = new InExpression(name, values);
			AddCriterion(inExpression);
			return this;
		}

		public QueryBuilder<T> In<K>(ICollection<K> values)
		{
			object[] arr = ToArray(values);
			AbstractCriterion inExpression = new InExpression(name, arr);
			AddCriterion(inExpression);
			return this;
		}

		public QueryBuilder<T> IsNotNull
		{
			get
			{
				AbstractCriterion notNullExpression = new NotNullExpression(name);
				AddCriterion(notNullExpression);
				return this;
			}
		}

		public QueryBuilder<T> IsNull
		{
			get
			{
				AbstractCriterion nullExpression = new NullExpression(name);
				AddCriterion(nullExpression);
				return this;
			}
		}

		#region Operator Overloading Magic

		public static QueryBuilder<T> operator ==(QueryBuilder<T> expr, object other)
		{
			return expr.Eq(other);
		}

		public static QueryBuilder<T> operator !=(QueryBuilder<T> expr, object other)
		{
			return expr.NotEq(other);
		}

		public static QueryBuilder<T> operator &(QueryBuilder<T> lhs, QueryBuilder<T> rhs)
		{
			QueryBuilder<T> combined = new QueryBuilder<T>(lhs.name, null);
			combined.children.Add(lhs);
			combined.children.Add(rhs);
			return combined;
		}

		public static QueryBuilder<T> operator |(QueryBuilder<T> lhs, QueryBuilder<T> rhs)
		{
			if (lhs.assoicationPath != rhs.assoicationPath)
			{
				throw new InvalidOperationException(
					string.Format(
						@"OR attempted between {0} and {1}.
You can't OR between two Query parts that belong to different assoications.
Use HQL for this functionality...",
						lhs.assoicationPath,
						rhs.assoicationPath));
			}

			QueryBuilder<T> combined = new QueryBuilder<T>(lhs.name, null);
			Conjunction lhs_conjunction = Expression.Conjunction();
			Conjunction rhs_conjunction = Expression.Conjunction();
			foreach (ICriterion criterion in lhs.criterions)
			{
				lhs_conjunction.Add(criterion);
			}
			foreach (ICriterion criterion in rhs.criterions)
			{
				rhs_conjunction.Add(criterion);
			}
			combined.criterions.Add(Expression.Or(lhs_conjunction, rhs_conjunction));
			return combined;
		}

		public static bool operator true(QueryBuilder<T> exp)
		{
			return false;
		}

		public static bool operator false(QueryBuilder<T> exp)
		{
			return false;
		}

		public static implicit operator DetachedCriteria(QueryBuilder<T> expr)
		{
			DetachedCriteria detachedCriteria = DetachedCriteria.For<T>();
			Dictionary<string, ICollection<ICriterion>> criterionsByAssociation = new Dictionary<string, ICollection<ICriterion>>();
			expr.AddByAssoicationPath(criterionsByAssociation);

			foreach (KeyValuePair<string, ICollection<ICriterion>> pair in criterionsByAssociation)
			{
				DetachedCriteria temp = detachedCriteria;
				if (pair.Key != "this")
					temp = detachedCriteria.CreateCriteria(pair.Key);
				foreach (ICriterion criterion in pair.Value)
				{
					temp.Add(criterion);
				}
			}
			return detachedCriteria;
		}

		#endregion

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("You can't use Equals()! Use Eq()");
		}


		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		protected static object[] ToArray(ICollection values)
		{
			object[] arr = new object[values.Count];
			values.CopyTo(arr, 0);
			return arr;
		}

		protected static string BackTrackAssoicationPath(string assoicationPath)
		{
			int lastIndexOfPeriod = assoicationPath.LastIndexOf('.');
			if (lastIndexOfPeriod == -1)//this mean we are on "this", no need to do anything
				return assoicationPath;
			return assoicationPath.Substring(0, lastIndexOfPeriod);
			
		}

		protected static object[] ToArray<K>(ICollection<K> values)
		{
			K[] arr = new K[values.Count];
			values.CopyTo(arr, 0);
			return ToArray((ICollection)arr);//need this to convert to the object[] instead of K[]
		}

		private void AddByAssoicationPath(IDictionary<string, ICollection<ICriterion>> criterionsByAssociation)
		{
			if (criterionsByAssociation.ContainsKey(assoicationPath) == false)
				criterionsByAssociation.Add(assoicationPath, new List<ICriterion>());
			foreach (ICriterion criterion in criterions)
			{
				criterionsByAssociation[assoicationPath].Add(criterion);
			}
			foreach (QueryBuilder<T> child in children)
			{
				child.AddByAssoicationPath(criterionsByAssociation);
			}
		}
	}

	public class PropertyQueryBuilder<T> : QueryBuilder<T>
	{
		public PropertyQueryBuilder(string name, string assoicationPath)
			: base(name, assoicationPath)
		{
		}

		public QueryBuilder<T> Between(object lo, object hi)
		{
			AbstractCriterion betweenExpression = new BetweenExpression(name, lo, hi);
			AddCriterion(betweenExpression);
			return this;
		}

		public QueryBuilder<T> EqProperty(string otherPropertyName)
		{
			AbstractCriterion eqPropertyExpression = new EqPropertyExpression(name, otherPropertyName);
			AddCriterion(eqPropertyExpression);
			return this;
		}


		public QueryBuilder<T> Ge(object value)
		{
			AbstractCriterion geExpression = new GeExpression(name, value);
			AddCriterion(geExpression);
			return this;
		}

		public QueryBuilder<T> Gt(object value)
		{
			AbstractCriterion gtExpression = new GtExpression(name, value);
			AddCriterion(gtExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(object value)
		{
			AbstractCriterion insensitiveLikeExpression = new InsensitiveLikeExpression(name, value);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(string value, MatchMode matchMode)
		{
			AbstractCriterion insensitiveLikeExpression = new InsensitiveLikeExpression(name, value, matchMode);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}
		public QueryBuilder<T> Le(object value)
		{
			AbstractCriterion leExpression = new LeExpression(name, value);
			AddCriterion(leExpression);
			return this;
		}

		public QueryBuilder<T> LeProperty(string otherPropertyName)
		{
			AbstractCriterion lePropertyExpression = new LePropertyExpression(name, otherPropertyName);
			AddCriterion(lePropertyExpression);
			return this;
		}

		public QueryBuilder<T> Like(object value)
		{
			AbstractCriterion likeExpression = new LikeExpression(name, value);
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Like(string value, MatchMode matchMode)
		{
			AbstractCriterion likeExpression = new LikeExpression(name, value, matchMode);
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Lt(object value)
		{
			AbstractCriterion ltExpression = new LtExpression(name, value);
			AddCriterion(ltExpression);
			return this;
		}

		public QueryBuilder<T> LtProperty(string otherPropertyName)
		{
			AbstractCriterion ltPropertyExpression = new LtPropertyExpression(name, otherPropertyName);
			AddCriterion(ltPropertyExpression);
			return this;
		}

		public static QueryBuilder<T> operator >(PropertyQueryBuilder<T> expr, object other)
		{
			return expr.Gt(other);
		}

		public static QueryBuilder<T> operator <(PropertyQueryBuilder<T> expr, object other)
		{
			return expr.Lt(other);
		}

		public static QueryBuilder<T> operator >=(PropertyQueryBuilder<T> expr, object other)
		{
			return expr.Ge(other);
		}

		public static QueryBuilder<T> operator <=(PropertyQueryBuilder<T> expr, object other)
		{
			return expr.Le(other);
		}
	}
	
	public class OrderByClause
	{
		bool ascending = true;
		string name;

		public OrderByClause(string name)
		{
			this.name = name;
		}

		public OrderByClause Asc
		{
			get
			{
				ascending = true;
				return this;
			}
		}

		public OrderByClause Desc
		{
			get
			{
				ascending = false;
				return this;
			}
		}

		public static implicit operator Order(OrderByClause order)
		{
			return new Order(order.name, order.ascending);	
		}
	}
}