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




//Don't optimize the using statements in this file, as naming conflicts between 
//generated code and other (project) objects may occur.
using System.Collections.Generic;
using NHibernate.Criterion;

namespace QueryNamespace
{
	public partial class QueryBuilder<T>
	{
		protected string myName;
		/// <summary>
		/// This is needed so we can support
		/// Where.Comment.Post == post
		/// And it will turn to Expression.Eq("Post", post)  on the root rather than on a sub
		/// criteria
		/// </summary>
		protected string associationPath;
		protected bool backTrackAssociationsOnEquality;
		private readonly System.Collections.Generic.IList<OrderByClause> orderByClauses = new System.Collections.Generic.List<OrderByClause>();
		private readonly System.Collections.Generic.ICollection<QueryBuilder<T>> children = new Iesi.Collections.Generic.HashedSet<QueryBuilder<T>>();
		private readonly System.Collections.Generic.ICollection<NHibernate.Criterion.ICriterion> criterions = new System.Collections.Generic.List<NHibernate.Criterion.ICriterion>();
		private PropertyProjectionBuilder propertyProjection = null;
		public NHibernate.SqlCommand.JoinType joinType = NHibernate.SqlCommand.JoinType.InnerJoin;
		public NHibernate.FetchMode? fetchMode;
		protected QueryBuilder<T> myQueryParent;
		protected bool myShouldSkipJoinOnIdEquality = false;

		public delegate DetachedCriteria CreateDetachedCriteriaDelegate(string alias);
		public static CreateDetachedCriteriaDelegate CreateDetachedCriteria = DefaultCreateDetachedCriteria;

		public virtual bool ShouldSkipJoinOnIdEquality
		{
			get { return myShouldSkipJoinOnIdEquality; }
			set { myShouldSkipJoinOnIdEquality = value; }
		}

		public QueryBuilder()
			: this(null, typeof(T).Name, null)
		{
		}

		public QueryBuilder(string name)
			: this(null, name, null)
		{
		}

		public QueryBuilder(QueryBuilder<T> myQueryParent, string name, string associationPath, bool backTrackAssociationsOnEquality)
			: this(myQueryParent, name, associationPath)
		{
			this.backTrackAssociationsOnEquality = backTrackAssociationsOnEquality;
		}

		public QueryBuilder(QueryBuilder<T> myQueryParent, string name, string associationPath)
		{
			this.myQueryParent = myQueryParent;
			this.myName = name;
			this.associationPath = associationPath ?? "this";
			if (ReferenceEquals(myQueryParent, null) == false)//can't use != it is overloaded
				myQueryParent.children.Add(this);
		}

		public QueryBuilder<T> AddCriterion(NHibernate.Criterion.ICriterion criterion)
		{
			criterions.Add(criterion);
			return this;
		}

		public QueryBuilder<T> SetProjection(PropertyProjectionBuilder propertyProjection)
		{
			if (propertyProjection == null) throw new System.ArgumentNullException("propertyProjection");
			this.propertyProjection = propertyProjection;
			return this;

		}

		public QueryBuilder<T> AddOrder(OrderByClause orderByClause)
		{
			this.orderByClauses.Add(orderByClause);
			return this;
		}

		public IEnumerable<ICriterion> GetCriterionRecursive()
		{
			foreach (ICriterion criterion in criterions)
			{
				yield return criterion;
			}
			foreach (QueryBuilder<T> child in children)
			{
				foreach (ICriterion criterion in child.GetCriterionRecursive())
				{
					yield return criterion;
				}
			}
		}

		public QueryBuilder<T> AddOrder(params OrderByClause[] orderByClauses)
		{
			if (orderByClauses != null)
			{
				foreach (OrderByClause orderByClause in orderByClauses)
				{
					AddOrder(orderByClause);
				}
			}
			return this;

		}

		public QueryBuilder<T> Eq(object value)
		{
			NHibernate.Criterion.AbstractCriterion eq;
			if (value == null)
				eq = NHibernate.Criterion.Expression.IsNull(myName);
			else
				eq = NHibernate.Criterion.Expression.Eq(myName, value);
			QueryBuilder<T> self = this;
			if (backTrackAssociationsOnEquality)
			{
				self = new QueryBuilder<T>(this, myName, BackTrackAssociationPath(associationPath));
			}
			self.AddCriterion(eq);
			return this;
		}


		public QueryBuilder<T> NotEq(object value)
		{
			NHibernate.Criterion.AbstractCriterion eq;
			if (value == null)
				eq = NHibernate.Criterion.Expression.IsNotNull(myName);
			else
				eq = NHibernate.Criterion.Expression.Not(NHibernate.Criterion.Expression.Eq(myName, value));
			QueryBuilder<T> self = this;
			if (backTrackAssociationsOnEquality)
			{
				self = new QueryBuilder<T>(this, myName, BackTrackAssociationPath(associationPath));
			}
			self.AddCriterion(eq);
			return this;
		}

		public QueryBuilder<T> In<K>(params K[] values)
		{
			NHibernate.Criterion.AbstractCriterion inExpression = NHibernate.Criterion.Expression.In(myName, values);
			QueryBuilder<T> self = this;
			if (backTrackAssociationsOnEquality)
			{
				self = new QueryBuilder<T>(this, myName, BackTrackAssociationPath(associationPath));
			}
			self.AddCriterion(inExpression);
			return this;
		}

		public QueryBuilder<T> In(params object[] values)
		{
			In((System.Collections.ICollection)values);
			return this;
		}

		public QueryBuilder<T> In<K>(System.Collections.Generic.ICollection<K> values)
		{
			In(new System.Collections.Generic.List<K>(values).ToArray());
			return this;
		}

		public QueryBuilder<T> In<K>(System.Collections.Generic.IEnumerable<K> values)
		{
			In(new System.Collections.Generic.List<K>(values).ToArray());
			return this;
		}

		public QueryBuilder<T> IsNotNull
		{
			get
			{
				NHibernate.Criterion.AbstractCriterion notNullExpression = new NHibernate.Criterion.NotNullExpression(myName);
				QueryBuilder<T> self = this;
				if (backTrackAssociationsOnEquality)
				{
					self = new QueryBuilder<T>(this, myName, BackTrackAssociationPath(associationPath));
				}
				self.AddCriterion(notNullExpression);
				return this;
			}
		}

		public QueryBuilder<T> IsNull
		{
			get
			{
				NHibernate.Criterion.AbstractCriterion nullExpression = new NHibernate.Criterion.NullExpression(myName);
				QueryBuilder<T> self = this;
				if (backTrackAssociationsOnEquality)
				{
					self = new QueryBuilder<T>(this, myName, BackTrackAssociationPath(associationPath));
				}
				self.AddCriterion(nullExpression);
				return this;
			}
		}

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
			QueryBuilder<T> combined = new QueryBuilder<T>(null, lhs.myName, null);

			QueryBuilder<T> lhsRoot = lhs;
			while (ReferenceEquals(lhsRoot.myQueryParent, null) == false)
			{
				lhsRoot = lhsRoot.myQueryParent;
			}

			QueryBuilder<T> rhsRoot = rhs;
			while (ReferenceEquals(rhsRoot.myQueryParent, null) == false)
			{
				rhsRoot = rhsRoot.myQueryParent;
			}


			combined.children.Add(lhsRoot);
			combined.children.Add(rhsRoot);
			return combined;
		}

		public static QueryBuilder<T> operator !(QueryBuilder<T> other)
		{
			QueryBuilder<T> not = new QueryBuilder<T>(other, other.myName, null);
			
			NHibernate.Criterion.Conjunction conjunction = new NHibernate.Criterion.Conjunction();
			foreach (NHibernate.Criterion.ICriterion crit in other.GetCriterionRecursive())
			{
				conjunction.Add(crit);
			}
			other.criterions.Clear();
			other.children.Clear();
			not.AddCriterion(NHibernate.Criterion.Expression.Not(conjunction));
			return not;
		}

		public static QueryBuilder<T> operator |(QueryBuilder<T> lhs, QueryBuilder<T> rhs)
		{
			if (lhs.associationPath != rhs.associationPath)
			{
				throw new System.InvalidOperationException(
					string.Format(
						@"OR attempted between {0} and {1}.
You can't OR between two Query parts that belong to different associations.
Use HQL for this functionality...",
						lhs.associationPath,
						rhs.associationPath));
			}

			QueryBuilder<T> combined = new QueryBuilder<T>(null, lhs.myName, null);
			NHibernate.Criterion.Conjunction lhs_conjunction = NHibernate.Criterion.Expression.Conjunction();
			NHibernate.Criterion.Conjunction rhs_conjunction = NHibernate.Criterion.Expression.Conjunction();
			foreach (NHibernate.Criterion.ICriterion criterion in lhs.GetCriterionRecursive())
			{
				lhs_conjunction.Add(criterion);
			}
			foreach (NHibernate.Criterion.ICriterion criterion in rhs.GetCriterionRecursive())
			{
				rhs_conjunction.Add(criterion);
			}
			combined.criterions.Add(NHibernate.Criterion.Expression.Or(lhs_conjunction, rhs_conjunction));
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

		public static implicit operator NHibernate.Criterion.DetachedCriteria(QueryBuilder<T> expr)
		{
			return expr.ToDetachedCriteria(null);
		}

		public NHibernate.Criterion.DetachedCriteria ToDetachedCriteria()
		{
			return ToDetachedCriteria(null);
		}

		public NHibernate.Criterion.DetachedCriteria ToDetachedCriteria(string alias)
		{
			return ToDetachedCriteria(alias, null);
		}

		public DetachedCriteria ToDetachedCriteria(string alias, params OrderByClause[] sortOrders)
		{
			if (ReferenceEquals(myQueryParent, null) == false)//can't use != we overloaded that
			{
				return myQueryParent.ToDetachedCriteria(alias);
			}

			NHibernate.Criterion.DetachedCriteria detachedCriteria = CreateDetachedCriteria(alias);

			if (this.propertyProjection != null)
			{
				NHibernate.Criterion.ProjectionList projectionList = this.propertyProjection;
				detachedCriteria.SetProjection(projectionList);
			}

			System.Collections.Generic.Dictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>> criterionsByJoinTypeAndFetchMode =
				new System.Collections.Generic.Dictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>>();
			System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<NHibernate.Criterion.ICriterion>> criterionsByAssociation =
				new System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<NHibernate.Criterion.ICriterion>>();
			AddByAssociationPath(criterionsByAssociation, criterionsByJoinTypeAndFetchMode);
			System.Collections.Hashtable critTable = new System.Collections.Hashtable();
			critTable.Add("this", detachedCriteria);
			foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.ICollection<NHibernate.Criterion.ICriterion>> pair in criterionsByAssociation)
			{
				NHibernate.Criterion.DetachedCriteria temp = detachedCriteria;
				System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode> val;
				if (criterionsByJoinTypeAndFetchMode.TryGetValue(pair.Key, out val) == false)
					continue;
				if (pair.Key != "this")
				{
					temp = CreateCriteriaByPath(pair.Key, critTable, detachedCriteria, val.Key);
				}
				foreach (NHibernate.Criterion.ICriterion criterion in pair.Value)
				{
					temp.Add(criterion);
				}
			}

			System.Collections.Generic.List<OrderByClause> allOrderbyClauses = new System.Collections.Generic.List<OrderByClause>(this.orderByClauses);

			if (sortOrders != null)
			{
				allOrderbyClauses.AddRange(sortOrders);
			}

			foreach (OrderByClause sortOrder in allOrderbyClauses)
			{
				if (!string.IsNullOrEmpty(sortOrder.AssociationPath))
				{
					NHibernate.Criterion.DetachedCriteria tempOrder = CreateCriteriaByPath(sortOrder.AssociationPath, critTable, detachedCriteria, NHibernate.SqlCommand.JoinType.LeftOuterJoin);
					tempOrder.AddOrder(sortOrder);
				}
			}
			
			return detachedCriteria;
		}

		private static DetachedCriteria DefaultCreateDetachedCriteria(string alias)
		{
			if (string.IsNullOrEmpty(alias))
				return NHibernate.Criterion.DetachedCriteria.For(typeof(T));
			else
				return NHibernate.Criterion.DetachedCriteria.For(typeof(T), alias);
		}

		private static NHibernate.Criterion.DetachedCriteria CreateCriteriaByPath(string keyPath, System.Collections.Hashtable critTable, DetachedCriteria currentCriteria, NHibernate.SqlCommand.JoinType joinType)
		{
			string assPath = keyPath;
			string[] split = assPath.Split('.');
			for (int i = 0; i < split.Length; i++)
			{
				if (critTable.ContainsKey(assPath))
					currentCriteria = (NHibernate.Criterion.DetachedCriteria)critTable[assPath];
				else
					assPath = BackTrackAssociationPath(assPath);
			}
			if (currentCriteria != null)
			{
				string remainingPath = keyPath.Replace(assPath, "");
				if (remainingPath.StartsWith("."))
					remainingPath = remainingPath.Substring(1);
				// create missing criteria
				string[] allPaths = remainingPath.Split('.');
				//if (allPaths.Length > 1)
				//    allPaths[allPaths.Length - 1] = ""; // remove property from association
				foreach (string s in allPaths)
				{
					if (!string.IsNullOrEmpty(s))
					{
						assPath = assPath + "." + s;
						currentCriteria = currentCriteria.CreateCriteria(s, joinType);
						critTable.Add(assPath, currentCriteria);
					}
				}
			}
			return currentCriteria;
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override bool Equals(object obj)
		{
			throw new System.InvalidOperationException("You can't use Equals()! Use Eq()");
		}


		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected static QueryBuilder<T> FromCriterion(NHibernate.Criterion.AbstractCriterion criterion,
			string name, string associationPath)
		{
			QueryBuilder<T> queryBuilder = new QueryBuilder<T>(null, name, associationPath);
			queryBuilder.AddCriterion(criterion);
			return queryBuilder;
		}


		protected static string BackTrackAssociationPath(string associationPath)
		{
			int lastIndexOfPeriod = associationPath.LastIndexOf('.');
			if (lastIndexOfPeriod == -1)//this mean we are on "this", no need to do anything
				return associationPath;
			return associationPath.Substring(0, lastIndexOfPeriod);

		}

		private void AddByAssociationPath(
			System.Collections.Generic.IDictionary<string, System.Collections.Generic.ICollection<NHibernate.Criterion.ICriterion>> criterionsByAssociation,
			System.Collections.Generic.IDictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>> criterionsByJoinTypeAndFetchMode)
		{
			if (criterionsByAssociation.ContainsKey(associationPath) == false)
				criterionsByAssociation.Add(associationPath, new System.Collections.Generic.List<NHibernate.Criterion.ICriterion>());
			if (criterionsByJoinTypeAndFetchMode.ContainsKey(associationPath) == false ||
				this.myQueryParent is CollectionQueryBuilder<T>)
			{
				//avoid adding temporary query builders if they are just there
				//to build the query, frex: Where.Post.Blog.Id, the Blog should not 
				//be counted.
				if (criterions.Count != 0 || this.fetchMode != null)
				{
					System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode> val = new System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>(
						this.joinType, this.fetchMode ?? NHibernate.FetchMode.Default);
					criterionsByJoinTypeAndFetchMode[associationPath] = val;
				}
			}

			foreach (NHibernate.Criterion.ICriterion criterion in criterions)
			{
				criterionsByAssociation[associationPath].Add(criterion);
			}
			foreach (QueryBuilder<T> child in children)
			{
				child.AddByAssociationPath(criterionsByAssociation, criterionsByJoinTypeAndFetchMode);
			}
		}
	}

	public partial class IdQueryBuilder<T> : QueryBuilder<T>
	{
		private string oldName;
		public IdQueryBuilder(QueryBuilder<T> myQueryParent, string name, string associationPath)
			: base(myQueryParent, name, associationPath)
		{
			this.oldName = name;
			associationPath = BackTrackParentPath(associationPath);
			this.myName = associationPath + "." + name;
			backTrackAssociationsOnEquality = true;
		}

		public override bool ShouldSkipJoinOnIdEquality
		{
			get { return base.ShouldSkipJoinOnIdEquality; }
			set 
			{ 
				base.ShouldSkipJoinOnIdEquality = value;
				if (ShouldSkipJoinOnIdEquality)
				{
					backTrackAssociationsOnEquality = false;
					this.myName = oldName;
				}
			}
		}

		private static string BackTrackParentPath(string associationPath)
		{
			int lastIndexOfPeriod = associationPath.LastIndexOf('.');
			if (lastIndexOfPeriod == -1)//this mean we are on "this", no need to do anything
				return associationPath;
			return associationPath.Substring(lastIndexOfPeriod + 1);
		}
	}

	public partial class CollectionQueryBuilder<T> : QueryBuilder<T>
	{
		public CollectionQueryBuilder(QueryBuilder<T> myQueryParent, string name, string associationPath)
			: base(myQueryParent, name, associationPath)
		{
			if (ReferenceEquals(myQueryParent, null) == false)//can't use != it is overloaded
			{
				if (associationPath == null || associationPath == "this")
				{
					throw new System.InvalidOperationException(
						"BUG: tried to create a nested query builder with no assoication path!");
				}
			}
		}

		public QueryBuilder<T> Exists(NHibernate.Criterion.DetachedCriteria criteria)
		{
			criteria = criteria.SetProjection(NHibernate.Criterion.Projections.Id());
			AddCriterion(NHibernate.Criterion.Subqueries.Exists(criteria));
			return this;
		}

		public QueryBuilder<T> NotExists(NHibernate.Criterion.DetachedCriteria criteria)
		{
			criteria = criteria.SetProjection(NHibernate.Criterion.Projections.Property("id"));
			AddCriterion(NHibernate.Criterion.Subqueries.NotExists(criteria));
			return this;
		}
	}

	public partial class PropertyQueryBuilder<T> : QueryBuilder<T>
	{
		public PropertyQueryBuilder(QueryBuilder<T> myQueryParent, string name, string associationPath)
			: base(myQueryParent, name, associationPath)
		{
		}

		public QueryBuilder<T> Between(object lo, object hi)
		{
			NHibernate.Criterion.AbstractCriterion betweenExpression = new NHibernate.Criterion.BetweenExpression(myName, lo, hi);
			AddCriterion(betweenExpression);
			return this;
		}

		public QueryBuilder<T> EqProperty(string otherPropertyName)
		{
			NHibernate.Criterion.AbstractCriterion eqPropertyExpression = new NHibernate.Criterion.EqPropertyExpression(myName, otherPropertyName);
			AddCriterion(eqPropertyExpression);
			return this;
		}


		public QueryBuilder<T> Ge(object value)
		{
			NHibernate.Criterion.ICriterion geExpression = NHibernate.Criterion.Expression.Ge(myName, value);
			AddCriterion(geExpression);
			return this;
		}

		public QueryBuilder<T> Gt(object value)
		{
			NHibernate.Criterion.ICriterion gtExpression = NHibernate.Criterion.Expression.Gt(myName, value);
			AddCriterion(gtExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(object value)
		{
			NHibernate.Criterion.AbstractCriterion insensitiveLikeExpression = new NHibernate.Criterion.InsensitiveLikeExpression(myName, value);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(string value, NHibernate.Criterion.MatchMode matchMode)
		{
			NHibernate.Criterion.AbstractCriterion insensitiveLikeExpression = new NHibernate.Criterion.InsensitiveLikeExpression(myName, value, matchMode);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}
		public QueryBuilder<T> Le(object value)
		{
			NHibernate.Criterion.ICriterion leExpression = NHibernate.Criterion.Expression.Le(myName, value);
			AddCriterion(leExpression);
			return this;
		}

		public QueryBuilder<T> LeProperty(string otherPropertyName)
		{
			NHibernate.Criterion.AbstractCriterion lePropertyExpression = new NHibernate.Criterion.LePropertyExpression(myName, otherPropertyName);
			AddCriterion(lePropertyExpression);
			return this;
		}

		public QueryBuilder<T> Like(object value)
		{
			NHibernate.Criterion.ICriterion likeExpression = NHibernate.Criterion.Expression.Like( myName, value );
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Like(string value, NHibernate.Criterion.MatchMode matchMode)
		{
			NHibernate.Criterion.AbstractCriterion likeExpression = new NHibernate.Criterion.LikeExpression(myName, value, matchMode);
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Lt(object value)
		{
			NHibernate.Criterion.ICriterion ltExpression = NHibernate.Criterion.Expression.Lt(myName, value);
			AddCriterion(ltExpression);
			return this;
		}

		public QueryBuilder<T> LtProperty(string otherPropertyName)
		{
			NHibernate.Criterion.AbstractCriterion ltPropertyExpression = new NHibernate.Criterion.LtPropertyExpression(myName, otherPropertyName);
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

	public partial class OrderByClause
	{
		bool ascending = true;
		string name;
		string associationPath;

		public OrderByClause(string name)
		{
			this.name = name;
			this.associationPath = "this";
		}

		public OrderByClause(string name, string path)
		{
			this.name = name;
			this.associationPath = path;
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


		public string AssociationPath
		{
			get { return associationPath; }
			set { associationPath = value; }
		}

		public static implicit operator Order(OrderByClause order)
		{
			return new Order(order.name, order.ascending);
		}
	}

	public partial class ProjectBy
	{
		public static NHibernate.Criterion.IProjection RowCount
		{
			get { return NHibernate.Criterion.Projections.RowCount(); }
		}

		public static NHibernate.Criterion.IProjection Id
		{
			get { return NHibernate.Criterion.Projections.Id(); }
		}

		public static NHibernate.Criterion.IProjection Distinct(NHibernate.Criterion.IProjection projection)
		{
			return NHibernate.Criterion.Projections.Distinct(projection);
		}

		public static NHibernate.Criterion.IProjection SqlProjection(string sql, string[] aliases, NHibernate.Type.IType[] types)
		{
			return NHibernate.Criterion.Projections.SqlProjection(sql, aliases, types);
		}

		public static NHibernate.Criterion.IProjection SqlGroupByProjection(string sql, string groupBy, string[] aliases, NHibernate.Type.IType[] types)
		{
			return NHibernate.Criterion.Projections.SqlGroupProjection(sql, groupBy, aliases, types);
		}
	}


	public partial class PropertyProjectionBuilder
	{
		protected string name;
		NHibernate.Criterion.ProjectionList list;

		public PropertyProjectionBuilder(string name)
		{
			this.name = name;
		}

		public PropertyProjectionBuilder(NHibernate.Criterion.IProjection projection)
		{
			list = NHibernate.Criterion.Projections.ProjectionList().Add(projection);
		}

		public PropertyProjectionBuilder Count
		{
			get { return new PropertyProjectionBuilder(NHibernate.Criterion.Projections.Count(name)); }
		}

		public PropertyProjectionBuilder DistinctCount
		{
			get { return new PropertyProjectionBuilder(NHibernate.Criterion.Projections.CountDistinct(name)); }
		}

		public PropertyProjectionBuilder Max
		{
			get { return new PropertyProjectionBuilder(NHibernate.Criterion.Projections.Max(name)); }
		}

		public PropertyProjectionBuilder Min
		{
			get { return new PropertyProjectionBuilder(NHibernate.Criterion.Projections.Min(name)); }
		}


		#region Operator Overloading Magic

		public static implicit operator NHibernate.Criterion.ProjectionList(PropertyProjectionBuilder projection)
		{
			if (projection.list != null)
				return projection.list;
			return NHibernate.Criterion.Projections.ProjectionList()
				.Add(NHibernate.Criterion.Projections.Property(projection.name));
		}

		public static PropertyProjectionBuilder operator &(PropertyProjectionBuilder lhs, PropertyProjectionBuilder rhs)
		{
			if (lhs.list != null)
			{
				if (rhs.list == null)
				{
					lhs.list.Add(NHibernate.Criterion.Projections.Property(rhs.name));
				}
				else
				{
					lhs.list.Add(rhs.list);
				}
				return lhs;
			}
			lhs.list = NHibernate.Criterion.Projections.ProjectionList()
				.Add(NHibernate.Criterion.Projections.Property(lhs.name))
				.Add(NHibernate.Criterion.Projections.Property(rhs.name));
			return lhs;
		}

		public static PropertyProjectionBuilder operator &(PropertyProjectionBuilder lhs, NHibernate.Criterion.IProjection rhs)
		{
			if (lhs.list != null)
			{
				lhs.list.Add(rhs);
			}
			lhs.list = NHibernate.Criterion.Projections.ProjectionList()
				.Add(rhs);
			return lhs;
		}

		public static PropertyProjectionBuilder operator &(NHibernate.Criterion.IProjection lhs, PropertyProjectionBuilder rhs)
		{
			if (rhs.list != null)
			{
				rhs.list.Add(lhs);
			}
			rhs.list = NHibernate.Criterion.Projections.ProjectionList()
				.Add(lhs);
			return rhs;
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

	public partial class NumericPropertyProjectionBuilder : PropertyProjectionBuilder
	{
		public NumericPropertyProjectionBuilder(string name) : base(name) { }

		public NHibernate.Criterion.IProjection Avg
		{
			get { return NHibernate.Criterion.Projections.Avg(name); }
		}

		public NHibernate.Criterion.IProjection Sum
		{
			get { return NHibernate.Criterion.Projections.Sum(name); }
		}

		public static implicit operator NHibernate.Criterion.PropertyProjection(NumericPropertyProjectionBuilder projection)
		{
			return NHibernate.Criterion.Projections.Property(projection.name);
		}
	}
}
