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
using NHibernate.Expression;

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
		private readonly System.Collections.Generic.ICollection<NHibernate.Expression.ICriterion> criterions = new System.Collections.Generic.List<NHibernate.Expression.ICriterion>();
		private PropertyProjectionBuilder propertyProjection = null;
		public NHibernate.SqlCommand.JoinType joinType = NHibernate.SqlCommand.JoinType.InnerJoin;
		public NHibernate.FetchMode? fetchMode;
		protected QueryBuilder<T> parent;
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

		public QueryBuilder(QueryBuilder<T> parent, string name, string associationPath, bool backTrackAssociationsOnEquality)
			: this(parent, name, associationPath)
		{
			this.backTrackAssociationsOnEquality = backTrackAssociationsOnEquality;
		}

		public QueryBuilder(QueryBuilder<T> parent, string name, string associationPath)
		{
			this.parent = parent;
			this.myName = name;
			this.associationPath = associationPath ?? "this";
			if (ReferenceEquals(parent, null) == false)//can't use != it is overloaded
				parent.children.Add(this);
		}

		public QueryBuilder<T> AddCriterion(NHibernate.Expression.ICriterion criterion)
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
			NHibernate.Expression.AbstractCriterion eq;
			if (value == null)
				eq = NHibernate.Expression.Expression.IsNull(myName);
			else
				eq = NHibernate.Expression.Expression.Eq(myName, value);
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
			NHibernate.Expression.AbstractCriterion eq;
			if (value == null)
				eq = NHibernate.Expression.Expression.IsNotNull(myName);
			else
				eq = NHibernate.Expression.Expression.Not(NHibernate.Expression.Expression.Eq(myName, value));
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
			NHibernate.Expression.AbstractCriterion inExpression = NHibernate.Expression.Expression.In(myName, values);
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
				NHibernate.Expression.AbstractCriterion notNullExpression = new NHibernate.Expression.NotNullExpression(myName);
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
				NHibernate.Expression.AbstractCriterion nullExpression = new NHibernate.Expression.NullExpression(myName);
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
			while (ReferenceEquals(lhsRoot.parent, null) == false)
			{
				lhsRoot = lhsRoot.parent;
			}

			QueryBuilder<T> rhsRoot = rhs;
			while (ReferenceEquals(rhsRoot.parent, null) == false)
			{
				rhsRoot = rhsRoot.parent;
			}


			combined.children.Add(lhsRoot);
			combined.children.Add(rhsRoot);
			return combined;
		}

		public static QueryBuilder<T> operator !(QueryBuilder<T> other)
		{
			QueryBuilder<T> not = new QueryBuilder<T>(other, other.myName, null);
			if (other.children.Count != 0)
			{
				throw new System.InvalidOperationException("Cannot use ! operator on complex queries");
			}
			NHibernate.Expression.Conjunction conjunction = new NHibernate.Expression.Conjunction();
			foreach (NHibernate.Expression.ICriterion crit in other.GetCriterionRecursive())
			{
				conjunction.Add(crit);
			}
			other.criterions.Clear();
			other.children.Clear();
			not.AddCriterion(NHibernate.Expression.Expression.Not(conjunction));
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
			NHibernate.Expression.Conjunction lhs_conjunction = NHibernate.Expression.Expression.Conjunction();
			NHibernate.Expression.Conjunction rhs_conjunction = NHibernate.Expression.Expression.Conjunction();
			foreach (NHibernate.Expression.ICriterion criterion in lhs.GetCriterionRecursive())
			{
				lhs_conjunction.Add(criterion);
			}
			foreach (NHibernate.Expression.ICriterion criterion in rhs.GetCriterionRecursive())
			{
				rhs_conjunction.Add(criterion);
			}
			combined.criterions.Add(NHibernate.Expression.Expression.Or(lhs_conjunction, rhs_conjunction));
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

		public static implicit operator NHibernate.Expression.DetachedCriteria(QueryBuilder<T> expr)
		{
			return expr.ToDetachedCriteria(null);
		}

		public NHibernate.Expression.DetachedCriteria ToDetachedCriteria()
		{
			return ToDetachedCriteria(null);
		}

		public NHibernate.Expression.DetachedCriteria ToDetachedCriteria(string alias)
		{
			return ToDetachedCriteria(alias, null);
		}

		public DetachedCriteria ToDetachedCriteria(string alias, params OrderByClause[] sortOrders)
		{
			if (ReferenceEquals(parent, null) == false)//can't use != we overloaded that
			{
				return parent.ToDetachedCriteria(alias);
			}

			NHibernate.Expression.DetachedCriteria detachedCriteria = CreateDetachedCriteria(alias);

			if (this.propertyProjection != null)
			{
				NHibernate.Expression.ProjectionList projectionList = this.propertyProjection;
				detachedCriteria.SetProjection(projectionList);
			}

			System.Collections.Generic.Dictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>> criterionsByJoinTypeAndFetchMode =
				new System.Collections.Generic.Dictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>>();
			System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<NHibernate.Expression.ICriterion>> criterionsByAssociation =
				new System.Collections.Generic.Dictionary<string, System.Collections.Generic.ICollection<NHibernate.Expression.ICriterion>>();
			AddByAssociationPath(criterionsByAssociation, criterionsByJoinTypeAndFetchMode);
			System.Collections.Hashtable critTable = new System.Collections.Hashtable();
			critTable.Add("this", detachedCriteria);
			foreach (System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.ICollection<NHibernate.Expression.ICriterion>> pair in criterionsByAssociation)
			{
				NHibernate.Expression.DetachedCriteria temp = detachedCriteria;
				System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode> val;
				if (criterionsByJoinTypeAndFetchMode.TryGetValue(pair.Key, out val) == false)
					continue;
				if (pair.Key != "this")
				{
					temp = CreateCriteriaByPath(pair.Key, critTable, detachedCriteria, val.Key);
				}
				foreach (NHibernate.Expression.ICriterion criterion in pair.Value)
				{
					temp.Add(criterion);
				}
			}
			if (sortOrders != null)
			{
				foreach (OrderByClause sortOrder in sortOrders)
				{
					if (!string.IsNullOrEmpty(sortOrder.AssociationPath))
					{
						NHibernate.Expression.DetachedCriteria tempOrder = CreateCriteriaByPath(sortOrder.AssociationPath, critTable, detachedCriteria, NHibernate.SqlCommand.JoinType.LeftOuterJoin);
						tempOrder.AddOrder(sortOrder);
					}
				}
			}
			return detachedCriteria;
		}

		private static DetachedCriteria DefaultCreateDetachedCriteria(string alias)
		{
			if (string.IsNullOrEmpty(alias))
				return NHibernate.Expression.DetachedCriteria.For(typeof(T));
			else
				return NHibernate.Expression.DetachedCriteria.For(typeof(T), alias);
		}

		private static NHibernate.Expression.DetachedCriteria CreateCriteriaByPath(string keyPath, System.Collections.Hashtable critTable, DetachedCriteria currentCriteria, NHibernate.SqlCommand.JoinType joinType)
		{
			string assPath = keyPath;
			string[] split = assPath.Split('.');
			for (int i = 0; i < split.Length; i++)
			{
				if (critTable.ContainsKey(assPath))
					currentCriteria = (NHibernate.Expression.DetachedCriteria)critTable[assPath];
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

		protected static QueryBuilder<T> FromCriterion(NHibernate.Expression.AbstractCriterion criterion,
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
			System.Collections.Generic.IDictionary<string, System.Collections.Generic.ICollection<NHibernate.Expression.ICriterion>> criterionsByAssociation,
			System.Collections.Generic.IDictionary<string, System.Collections.Generic.KeyValuePair<NHibernate.SqlCommand.JoinType, NHibernate.FetchMode>> criterionsByJoinTypeAndFetchMode)
		{
			if (criterionsByAssociation.ContainsKey(associationPath) == false)
				criterionsByAssociation.Add(associationPath, new System.Collections.Generic.List<NHibernate.Expression.ICriterion>());
			if (criterionsByJoinTypeAndFetchMode.ContainsKey(associationPath) == false ||
				this.parent is CollectionQueryBuilder<T>)
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

			foreach (NHibernate.Expression.ICriterion criterion in criterions)
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
		public IdQueryBuilder(QueryBuilder<T> parent, string name, string associationPath)
			: base(parent, name, associationPath)
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
		public CollectionQueryBuilder(QueryBuilder<T> parent, string name, string associationPath)
			: base(parent, name, associationPath)
		{
			if (ReferenceEquals(parent, null) == false)//can't use != it is overloaded
			{
				if (associationPath == null || associationPath == "this")
				{
					throw new System.InvalidOperationException(
						"BUG: tried to create a nested query builder with no assoication path!");
				}
			}
		}

		public QueryBuilder<T> Exists(NHibernate.Expression.DetachedCriteria criteria)
		{
			criteria = criteria.SetProjection(NHibernate.Expression.Projections.Property("id"));
			AddCriterion(NHibernate.Expression.Subqueries.Exists(criteria));
			return this;
		}

		public QueryBuilder<T> NotExists(NHibernate.Expression.DetachedCriteria criteria)
		{
			criteria = criteria.SetProjection(NHibernate.Expression.Projections.Property("id"));
			AddCriterion(NHibernate.Expression.Subqueries.NotExists(criteria));
			return this;
		}
	}

	public partial class PropertyQueryBuilder<T> : QueryBuilder<T>
	{
		public PropertyQueryBuilder(QueryBuilder<T> parent, string name, string associationPath)
			: base(parent, name, associationPath)
		{
		}

		public QueryBuilder<T> Between(object lo, object hi)
		{
			NHibernate.Expression.AbstractCriterion betweenExpression = new NHibernate.Expression.BetweenExpression(myName, lo, hi);
			AddCriterion(betweenExpression);
			return this;
		}

		public QueryBuilder<T> EqProperty(string otherPropertyName)
		{
			NHibernate.Expression.AbstractCriterion eqPropertyExpression = new NHibernate.Expression.EqPropertyExpression(myName, otherPropertyName);
			AddCriterion(eqPropertyExpression);
			return this;
		}


		public QueryBuilder<T> Ge(object value)
		{
			NHibernate.Expression.AbstractCriterion geExpression = new NHibernate.Expression.GeExpression(myName, value);
			AddCriterion(geExpression);
			return this;
		}

		public QueryBuilder<T> Gt(object value)
		{
			NHibernate.Expression.AbstractCriterion gtExpression = new NHibernate.Expression.GtExpression(myName, value);
			AddCriterion(gtExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(object value)
		{
			NHibernate.Expression.AbstractCriterion insensitiveLikeExpression = new NHibernate.Expression.InsensitiveLikeExpression(myName, value);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}

		public QueryBuilder<T> InsensitiveLike(string value, NHibernate.Expression.MatchMode matchMode)
		{
			NHibernate.Expression.AbstractCriterion insensitiveLikeExpression = new NHibernate.Expression.InsensitiveLikeExpression(myName, value, matchMode);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}
		public QueryBuilder<T> Le(object value)
		{
			NHibernate.Expression.AbstractCriterion leExpression = new NHibernate.Expression.LeExpression(myName, value);
			AddCriterion(leExpression);
			return this;
		}

		public QueryBuilder<T> LeProperty(string otherPropertyName)
		{
			NHibernate.Expression.AbstractCriterion lePropertyExpression = new NHibernate.Expression.LePropertyExpression(myName, otherPropertyName);
			AddCriterion(lePropertyExpression);
			return this;
		}

		public QueryBuilder<T> Like(object value)
		{
			NHibernate.Expression.AbstractCriterion likeExpression = new NHibernate.Expression.LikeExpression(myName, value);
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Like(string value, NHibernate.Expression.MatchMode matchMode)
		{
			NHibernate.Expression.AbstractCriterion likeExpression = new NHibernate.Expression.LikeExpression(myName, value, matchMode);
			AddCriterion(likeExpression);
			return this;
		}

		public QueryBuilder<T> Lt(object value)
		{
			NHibernate.Expression.AbstractCriterion ltExpression = new NHibernate.Expression.LtExpression(myName, value);
			AddCriterion(ltExpression);
			return this;
		}

		public QueryBuilder<T> LtProperty(string otherPropertyName)
		{
			NHibernate.Expression.AbstractCriterion ltPropertyExpression = new NHibernate.Expression.LtPropertyExpression(myName, otherPropertyName);
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
		public static NHibernate.Expression.IProjection RowCount
		{
			get { return NHibernate.Expression.Projections.RowCount(); }
		}

		public static NHibernate.Expression.IProjection Id
		{
			get { return NHibernate.Expression.Projections.Id(); }
		}

		public static NHibernate.Expression.IProjection Distinct(NHibernate.Expression.IProjection projection)
		{
			return NHibernate.Expression.Projections.Distinct(projection);
		}

		public static NHibernate.Expression.IProjection SqlProjection(string sql, string[] aliases, NHibernate.Type.IType[] types)
		{
			return NHibernate.Expression.Projections.SqlProjection(sql, aliases, types);
		}

		public static NHibernate.Expression.IProjection SqlGroupByProjection(string sql, string groupBy, string[] aliases, NHibernate.Type.IType[] types)
		{
			return NHibernate.Expression.Projections.SqlGroupProjection(sql, groupBy, aliases, types);
		}
	}


	public partial class PropertyProjectionBuilder
	{
		protected string name;
		NHibernate.Expression.ProjectionList list;

		public PropertyProjectionBuilder(string name)
		{
			this.name = name;
		}

		public PropertyProjectionBuilder(NHibernate.Expression.IProjection projection)
		{
			list = NHibernate.Expression.Projections.ProjectionList().Add(projection);
		}

		public PropertyProjectionBuilder Count
		{
			get { return new PropertyProjectionBuilder(NHibernate.Expression.Projections.Count(name)); }
		}

		public PropertyProjectionBuilder DistinctCount
		{
			get { return new PropertyProjectionBuilder(NHibernate.Expression.Projections.CountDistinct(name)); }
		}

		public PropertyProjectionBuilder Max
		{
			get { return new PropertyProjectionBuilder(NHibernate.Expression.Projections.Max(name)); }
		}

		public PropertyProjectionBuilder Min
		{
			get { return new PropertyProjectionBuilder(NHibernate.Expression.Projections.Min(name)); }
		}


		#region Operator Overloading Magic

		public static implicit operator NHibernate.Expression.ProjectionList(PropertyProjectionBuilder projection)
		{
			if (projection.list != null)
				return projection.list;
			return NHibernate.Expression.Projections.ProjectionList()
				.Add(NHibernate.Expression.Projections.Property(projection.name));
		}

		public static PropertyProjectionBuilder operator &(PropertyProjectionBuilder lhs, PropertyProjectionBuilder rhs)
		{
			if (lhs.list != null)
			{
				if (rhs.list == null)
				{
					lhs.list.Add(NHibernate.Expression.Projections.Property(rhs.name));
				}
				else
				{
					lhs.list.Add(rhs.list);
				}
				return lhs;
			}
			lhs.list = NHibernate.Expression.Projections.ProjectionList()
				.Add(NHibernate.Expression.Projections.Property(lhs.name))
				.Add(NHibernate.Expression.Projections.Property(rhs.name));
			return lhs;
		}

		public static PropertyProjectionBuilder operator &(PropertyProjectionBuilder lhs, NHibernate.Expression.IProjection rhs)
		{
			if (lhs.list != null)
			{
				lhs.list.Add(rhs);
			}
			lhs.list = NHibernate.Expression.Projections.ProjectionList()
				.Add(rhs);
			return lhs;
		}

		public static PropertyProjectionBuilder operator &(NHibernate.Expression.IProjection lhs, PropertyProjectionBuilder rhs)
		{
			if (rhs.list != null)
			{
				rhs.list.Add(lhs);
			}
			rhs.list = NHibernate.Expression.Projections.ProjectionList()
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

		public NHibernate.Expression.IProjection Avg
		{
			get { return NHibernate.Expression.Projections.Avg(name); }
		}

		public NHibernate.Expression.IProjection Sum
		{
			get { return NHibernate.Expression.Projections.Sum(name); }
		}

		public static implicit operator NHibernate.Expression.PropertyProjection(NumericPropertyProjectionBuilder projection)
		{
			return NHibernate.Expression.Projections.Property(projection.name);
		}
	}
}
