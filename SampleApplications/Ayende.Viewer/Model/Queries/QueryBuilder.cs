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
using NHibernate.Expression;
using NHibernate.Type;

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

		protected void AddCriterion(ICriterion criterion)
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
			if (backTrackAssoicationsOnEquality)
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
			QueryBuilder<T> self = this;
			if (backTrackAssoicationsOnEquality)
			{
				self = new QueryBuilder<T>(name, BackTrackAssoicationPath(assoicationPath));
				children.Add(self);
			}
			self.AddCriterion(inExpression);
			return this;
		}

		public QueryBuilder<T> In(params object[] values)
		{
			In((ICollection)values);
			return this;
		}

		public QueryBuilder<T> In<K>(ICollection<K> values)
		{
			In((ICollection)values);
			return this;
		}

		public QueryBuilder<T> In<K>(IEnumerable<K> values)
		{
			In((ICollection)new List<K>(values));
			return this;
		}

		public QueryBuilder<T> IsNotNull
		{
			get
			{
				AbstractCriterion notNullExpression = new NotNullExpression(name);
				QueryBuilder<T> self = this;
				if (backTrackAssoicationsOnEquality)
				{
					self = new QueryBuilder<T>(name, BackTrackAssoicationPath(assoicationPath));
					children.Add(self);
				}
				self.AddCriterion(notNullExpression);
				return this;
			}
		}

		public QueryBuilder<T> IsNull
		{
			get
			{
				AbstractCriterion nullExpression = new NullExpression(name);
				QueryBuilder<T> self = this;
				if (backTrackAssoicationsOnEquality)
				{
					self = new QueryBuilder<T>(name, BackTrackAssoicationPath(assoicationPath));
					children.Add(self);
				}
				self.AddCriterion(nullExpression);
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

		protected static QueryBuilder<T> FromCriterion(AbstractCriterion criterion,
			string name, string assoicationPath)
		{
			QueryBuilder<T> queryBuilder = new QueryBuilder<T>(name, assoicationPath);
			queryBuilder.AddCriterion(criterion);
			return queryBuilder;
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

	public partial class ProjectBy
	{
		public static IProjection RowCount
		{
			get { return Projections.RowCount(); }
		}

		public static IProjection Id
		{
			get { return Projections.Id(); }
		}

		public static IProjection Distinct(IProjection projection)
		{
			return Projections.Distinct(projection);
		}

		public static IProjection SqlProjection(string sql, string[] aliases, IType[] types)
		{
			return Projections.SqlProjection(sql, aliases, types);
		}

		public static IProjection SqlGroupByProjection(string sql, string groupBy, string[] aliases, IType[] types)
		{
			return Projections.SqlGroupProjection(sql, groupBy, aliases, types);
		}
	}


	public class PropertyProjectionBuilder
	{
		protected string name;
		ProjectionList list;

		public PropertyProjectionBuilder(string name)
		{
			this.name = name;
		}

		public IProjection Count
		{
			get { return Projections.Count(name); }
		}

		public IProjection DistinctCount
		{
			get { return Projections.CountDistinct(name); }
		}

		public IProjection Max
		{
			get { return Projections.Max(name); }
		}

		public IProjection Min
		{
			get { return Projections.Min(name); }
		}


		#region Operator Overloading Magic

		public static implicit operator ProjectionList(PropertyProjectionBuilder projection)
		{
			if (projection.list != null)
				return projection.list;
			return Projections.ProjectionList()
				.Add(Projections.Property(projection.name));
		}

		public static PropertyProjectionBuilder operator &(PropertyProjectionBuilder lhs, PropertyProjectionBuilder rhs)
		{
			if (lhs.list != null)
			{
				if (rhs.list == null)
				{
					lhs.list.Add(Projections.Property(rhs.name));
				}
				else
				{
					lhs.list.Add(rhs.list);
				}
				return lhs;
			}
			lhs.list = Projections.ProjectionList()
				.Add(Projections.Property(lhs.name))
				.Add(Projections.Property(rhs.name));
			return lhs;
		}


		public static bool operator true(PropertyProjectionBuilder exp)
		{
			return false;
		}

		public static bool operator false(PropertyProjectionBuilder exp)
		{
			return false;
		}

		#endregion

	}

	public class NumericPropertyProjectionBuilder : PropertyProjectionBuilder
	{
		public NumericPropertyProjectionBuilder(string name) : base(name) { }

		public IProjection Avg
		{
			get { return Projections.Avg(name); }
		}

		public IProjection Sum
		{
			get { return Projections.Sum(name); }
		}

		public static implicit operator PropertyProjection(NumericPropertyProjectionBuilder projection)
		{
			return Projections.Property(projection.name);
		}
	}
}
