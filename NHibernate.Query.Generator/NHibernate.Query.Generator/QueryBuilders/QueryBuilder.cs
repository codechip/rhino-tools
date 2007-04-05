using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using NHibernate;
using NHibernate.Expression;
using NHibernate.Type;

namespace Query
{

	#region QueryBuilder<T>

	public class QueryBuilder<T>
	{
		#region Fields

		protected internal WhereClause<T> where;
		protected internal ProjectionClauseProperty<T> projection;
		protected internal OrderByClauseProperty<T> order;
		protected internal ICollection<ICriterion> subqueries = new List<ICriterion>();
		protected internal bool whereHasValue;
		protected internal bool projectionHasValue;
		protected internal bool orderHasValue;

		#endregion

		#region Builder Methods

		#region Append Where Methods

		public virtual QueryBuilder<T> Where(WhereClause<T> whereClause)
		{
			if (where)
				where = where && whereClause;
			else
				where = whereClause;
			whereHasValue = true;
			return this;
		}

		public virtual QueryBuilder<T> And(WhereClause<T> whereClause)
		{
			return Where(whereClause);
		}

		public virtual QueryBuilder<T> Or(WhereClause<T> whereClause)
		{
			if (!whereHasValue)
				return Where(whereClause);
			where = where || whereClause;
			return this;
		}

		#endregion

		#region Append Projection Methods

		public virtual QueryBuilder<T> ProjectBy(ProjectionClauseProperty<T> projectByClause)
		{
			if (!projectionHasValue)
				projection = projectByClause;
			else
				projection = projection && projectByClause;
			projectionHasValue = true;
			return this;
		}



		#endregion

		#region Append OrderBy Methods

		public virtual QueryBuilder<T> Order(OrderByClauseProperty<T> orderByClause)
		{
			if (!orderHasValue)
				order = orderByClause;
			else
				order = order && orderByClause;
			orderHasValue = true;
			return this;
		}

		public virtual QueryBuilder<T> NotExists(Subquery subQuery)
		{
			subqueries.Add(Subqueries.NotExists(subQuery));
			return this;
		}

		public virtual QueryBuilder<T> Exists(Subquery subQuery)
		{
			subqueries.Add(Subqueries.Exists(subQuery));
			return this;
		}

		public CustomResultQueryBuilder<T, RT> WithResultType<RT>()
		{
			return new CustomResultQueryBuilder<T, RT>(this);
		}

		public CustomResultQueryBuilder<T, RT> WithResultType<RT>(ConstructorInfo ci)
		{
			return new CustomResultQueryBuilder<T, RT>(this, ci);
		}

		#endregion

		#endregion

		#region Reset Query Builder

		public virtual void ResetSubqueries()
		{
			subqueries.Clear();
		}

		public virtual void ResetOrderBy()
		{
			order = null;
			orderHasValue = false;
		}

		public virtual void ResetProjections()
		{
			projection = null;
			projectionHasValue = false;
		}

		public virtual void ResetWhere()
		{
			where = null;
			whereHasValue = false;
		}

		public virtual void Reset()
		{
			ResetWhere();
			ResetProjections();
			ResetOrderBy();
			ResetSubqueries();
		}

		#endregion

		#region Casting Magic

		public static implicit operator DetachedCriteria(QueryBuilder<T> expr)
		{
			return expr.ToDetachedCriteria(null);
		}

		public static implicit operator Subquery(QueryBuilder<T> expr)
		{
			return new Subquery(expr.ToDetachedCriteria(null));
		}

		public virtual DetachedCriteria ToDetachedCriteria()
		{
			return ToDetachedCriteria(null);
		}

		public virtual DetachedCriteria ToDetachedCriteria(string alias)
		{
			DetachedCriteria detachedCriteria;
			if (String.IsNullOrEmpty(alias))
				detachedCriteria = DetachedCriteria.For(typeof(T));
			else
				detachedCriteria = DetachedCriteria.For(typeof(T), alias);

			IDictionary<string, DetachedCriteria> criterias = new Dictionary<string, DetachedCriteria>();

			if (whereHasValue)
				where.BuildQuery(detachedCriteria, criterias);

			if (projectionHasValue)
				projection.BuildQuery(detachedCriteria, criterias);

			if (orderHasValue)
				order.BuildQuery(detachedCriteria, criterias);

			foreach (ICriterion c in subqueries)
				detachedCriteria.Add(c);

			return detachedCriteria;
		}

		#endregion
	}

	public class CustomResultQueryBuilder<T, RT> : QueryBuilder<T>
	{
		private ConstructorInfo ci;

		protected internal CustomResultQueryBuilder(QueryBuilder<T> inner)
			: this(inner, typeof(RT).GetConstructors()[0])
		{
		}

		protected internal CustomResultQueryBuilder(QueryBuilder<T> inner, ConstructorInfo ci)
		{
			this.order = inner.order;
			this.projection = inner.projection;
			this.where = inner.where;
			this.orderHasValue = inner.orderHasValue;
			this.projectionHasValue = inner.projectionHasValue;
			this.whereHasValue = inner.whereHasValue;
			this.subqueries = inner.subqueries;
			this.ci = ci;
		}

		public override DetachedCriteria ToDetachedCriteria(string alias)
		{
			return base.ToDetachedCriteria(alias).SetResultTransformer(
				new NHibernate.Transform.AliasToBeanConstructorResultTransformer(ci));
		}

		public IList<RT> List(ISession session)
		{
			return ToDetachedCriteria().GetExecutableCriteria(session).List<RT>();
		}

		public RT UniqueResult(ISession session)
		{
			return ToDetachedCriteria().GetExecutableCriteria(session).UniqueResult<RT>();
		}
	}

	#endregion

	#region QueryPart

	public abstract class QueryPart
	{
		#region Fields

		protected internal string name;
		protected internal bool backTrack;
		protected internal string associationPath;

		#endregion

		#region ctor

		public QueryPart(string name, string associationPath)
			: this(name, associationPath, false)
		{
		}

		public QueryPart(string name, string associationPath, bool backTrack)
		{
			this.name = name;
			this.associationPath = associationPath ?? "this";
			this.backTrack = backTrack;
		}

		#endregion

		#region Properties

		protected internal virtual string Name
		{
			get { return name; }
		}

		protected internal virtual string Alias
		{
			get { return MakeAliasFromAssociationPath(associationPath); }
		}

		protected internal virtual string AliasAndName
		{
			get { return String.IsNullOrEmpty(Alias) ? Name : Alias + "." + Name; }
		}

		protected internal virtual bool IsOnRoot
		{
			get { return String.IsNullOrEmpty(Alias); }
		}

		#endregion

		#region Utility Methods

		protected static string MakeAliasFromAssociationPath(string associationPath)
		{
			if (String.IsNullOrEmpty(associationPath))
				return String.Empty;
			if (associationPath.Equals("this", StringComparison.InvariantCultureIgnoreCase))
				return String.Empty;
			if (associationPath.StartsWith("this.", StringComparison.InvariantCultureIgnoreCase))
				return associationPath.Substring(associationPath.IndexOf(".") + 1);
			if (associationPath.StartsWith("."))
				return associationPath.Substring(1);
			return associationPath;
		}

		#endregion
	}

	#endregion

	#region QueryClause<T>

	public abstract class QueryClause<T> : QueryPart
	{
		#region ctor

		public QueryClause(string name, string associationPath, bool backTrack)
			: base(name, associationPath, backTrack)
		{
		}

		public QueryClause(string name, string associationPath)
			: this(name, associationPath, false)
		{
		}

		#endregion

		/// <summary>
		/// Adds a new criteria using the Alias if the criteria does not already exist.
		/// </summary>
		/// <param name="rootCriteria"></param>
		/// <param name="criterias"></param>
		protected virtual void EnsureCriteriaExistsForAlias(DetachedCriteria rootCriteria,
																												IDictionary<string, DetachedCriteria> criterias)
		{
			if (backTrack)
				return;
			if (IsOnRoot)
				return;
			if (criterias.ContainsKey(Alias) == false)
				criterias.Add(Alias, rootCriteria.CreateCriteria(Alias, Alias));
		}

		protected static string BackTrackAssociationPath(string associationPath)
		{
			int lastIndexOfPeriod = associationPath.LastIndexOf('.');
			if (lastIndexOfPeriod == -1) //this mean we are on "this", no need to do anything
				return associationPath;
			return associationPath.Substring(0, lastIndexOfPeriod);
		}

		public DetachedCriteria ToDetachedCriteria()
		{
			return ToDetachedCriteria(null);
		}

		public virtual DetachedCriteria ToDetachedCriteria(string alias)
		{
			DetachedCriteria detachedCriteria;
			if (String.IsNullOrEmpty(alias))
				detachedCriteria = DetachedCriteria.For(typeof(T));
			else
				detachedCriteria = DetachedCriteria.For(typeof(T), alias);

			Dictionary<string, DetachedCriteria> criterias = new Dictionary<string, DetachedCriteria>();
			BuildQuery(detachedCriteria, criterias);
			return detachedCriteria;
		}

		public virtual void BuildQuery(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias)
		{
			EnsureCriteriaExistsForAlias(rootCriteria, criterias);
			AddCriteria(rootCriteria, criterias);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expr"></param>
		/// <returns></returns>
		/// <remarks>Should be implicit but breaks active record integration with order by clauses.</remarks>
		public static explicit operator DetachedCriteria(QueryClause<T> expr)
		{
		  return expr.ToDetachedCriteria(null);
		}

		protected abstract void AddCriteria(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias);
	}

	#endregion

	#region SimpleClause<T>

	public abstract class SimpleClause<T, CT> : QueryClause<T> where CT : SimpleClause<T, CT>
	{
		#region Fields

		protected internal List<CT> clauses;

		#endregion

		#region ctor

		protected SimpleClause(string name, string associationPath, bool backTrack)
			: base(name, associationPath, backTrack)
		{
		}

		protected SimpleClause(string name, string associationPath)
			: base(name, associationPath)
		{
		}

		#endregion

		protected internal virtual void AddOne(CT one)
		{
			if (clauses == null)
				clauses = new List<CT>();
			clauses.Add(one);
			one.clauses = null;
		}

		protected internal virtual void AddRange(CT one)
		{
			if (clauses == null)
				clauses = new List<CT>();
			clauses.AddRange(one.clauses);
			one.clauses = null;
		}

		protected internal static CT Combine(CT lhs, CT rhs, CT newClause)
		{
			if (lhs.clauses == null || lhs.clauses.Count == 0)
				lhs.AddOne(newClause);

			if (rhs.clauses == null || rhs.clauses.Count == 0)
				lhs.AddOne(rhs);
			else
				lhs.AddRange(rhs);

			return lhs;
		}
	}

	#endregion

	#region WhereClause<T>

	public partial class WhereClause<T> : QueryClause<T>
	{
		private ICollection<ICriterion> criterions = new List<ICriterion>();
		protected List<WhereClause<T>> children = new List<WhereClause<T>>();

		public WhereClause(string name, string associationPath, bool backTrackAssociationsOnEquality)
			: base(name, associationPath, backTrackAssociationsOnEquality)
		{
		}

		public WhereClause(string name, string associationPath)
			: base(name, associationPath)
		{
		}

		protected void AddCriterion(ICriterion criterion)
		{
			criterions.Add(criterion);
		}

		protected void AddChild(WhereClause<T> child)
		{
			if (children == null)
				children = new List<WhereClause<T>>();
			children.Add(child);
		}

		protected override void AddCriteria(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias)
		{
			foreach (ICriterion criterion in criterions)
			{
				if (IsOnRoot)
					rootCriteria.Add(criterion);
				else
					criterias[Alias].Add(criterion);
			}

			if (children != null)
				foreach (WhereClause<T> child in children)
					child.BuildQuery(rootCriteria, criterias);
		}

		#region Expressions

		public WhereClause<T> Eq(object value)
		{
			AbstractCriterion eq;
			if (value == null)
				eq = Expression.IsNull(name);
			else
				eq = Expression.Eq(name, value);
			WhereClause<T> self = this;
			if (backTrack)
			{
				self = new WhereClause<T>(name, BackTrackAssociationPath(associationPath));
				AddChild(self);
			}
			self.AddCriterion(eq);
			return this;
		}


		public WhereClause<T> NotEq(object value)
		{
			AbstractCriterion eq;
			if (value == null)
				eq = Expression.IsNotNull(name);
			else
				eq = Expression.Not(Expression.Eq(name, value));
			WhereClause<T> self = this;
			if (backTrack)
			{
				self = new WhereClause<T>(name, BackTrackAssociationPath(associationPath));
				AddChild(self);
			}
			self.AddCriterion(eq);
			return this;
		}

		public WhereClause<T> In<K>(params K[] values)
		{
			AbstractCriterion inExpression = Expression.In(name, values);
			WhereClause<T> self = this;
			if (backTrack)
			{
				self = new WhereClause<T>(name, BackTrackAssociationPath(associationPath));
				AddChild(self);
			}
			self.AddCriterion(inExpression);
			return this;
		}

		public WhereClause<T> In(params object[] values)
		{
			In((ICollection)values);
			return this;
		}

		public WhereClause<T> In<K>(ICollection<K> values)
		{
			In(new List<K>(values).ToArray());
			return this;
		}

		public WhereClause<T> In<K>(IEnumerable<K> values)
		{
			In(new List<K>(values).ToArray());
			return this;
		}

		public WhereClause<T> IsNotNull
		{
			get
			{
				AbstractCriterion notNullExpression = new NotNullExpression(name);
				WhereClause<T> self = this;
				if (backTrack)
				{
					self = new WhereClause<T>(name, BackTrackAssociationPath(associationPath));
					AddChild(self);
				}
				self.AddCriterion(notNullExpression);
				return this;
			}
		}

		public WhereClause<T> IsNull
		{
			get
			{
				AbstractCriterion nullExpression = new NullExpression(name);
				WhereClause<T> self = this;
				if (backTrack)
				{
					self = new WhereClause<T>(name, BackTrackAssociationPath(associationPath));
					AddChild(self);
				}
				self.AddCriterion(nullExpression);
				return this;
			}
		}

		#endregion Expressions

		#region Operator Overloading Magic

		public static WhereClause<T> operator ==(WhereClause<T> expr, object other)
		{
			return expr.Eq(other);
		}

		public static WhereClause<T> operator !=(WhereClause<T> expr, object other)
		{
			return expr.NotEq(other);
		}

		public static WhereClause<T> operator &(WhereClause<T> lhs, WhereClause<T> rhs)
		{
			WhereClause<T> combined = new WhereClause<T>(lhs.name, null);
			combined.AddChild(lhs);
			combined.AddChild(rhs);
			return combined;
		}

		public static WhereClause<T> operator !(WhereClause<T> other)
		{
			WhereClause<T> not = new WhereClause<T>(other.name, null);
			if (other.children == null || other.children.Count != 0)
			{
				throw new InvalidOperationException("Cannot use ! operator on complex queries");
			}
			Conjunction conjunction = new Conjunction();
			foreach (ICriterion crit in other.criterions)
			{
				conjunction.Add(crit);
			}
			not.AddCriterion(Expression.Not(conjunction));
			return not;
		}

		public static WhereClause<T> operator |(WhereClause<T> lhs, WhereClause<T> rhs)
		{
			if (lhs.associationPath != rhs.associationPath)
			{
				throw new InvalidOperationException(
					string.Format(
						@"OR attempted between {0} and {1}.
You can't OR between two Query parts that belong to different associations.
Use HQL for this functionality...",
						lhs.associationPath,
						rhs.associationPath));
			}

			WhereClause<T> combined = new WhereClause<T>(lhs.name, null);
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

		public static implicit operator DetachedCriteria(WhereClause<T> expr)
		{
			return expr.ToDetachedCriteria(null);
		}

		public static bool operator true(WhereClause<T> exp)
		{
			return false;
		}

		public static bool operator false(WhereClause<T> exp)
		{
			return false;
		}

		#endregion

		[Browsable(false)]
		[Localizable(false)]
		[Obsolete("You can't use Equals()! Use Eq()", true)]
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("You can't use Equals()! Use Eq()");
		}

		[Browsable(false)]
		[Localizable(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected static WhereClause<T> FromCriterion(AbstractCriterion criterion,
																									string name, string associationPath)
		{
			WhereClause<T> whereClause = new WhereClause<T>(name, associationPath);
			whereClause.AddCriterion(criterion);
			return whereClause;
		}
	}

	#endregion

	#region WhereClauseProperty<T>

	public partial class WhereClauseProperty<T> : WhereClause<T>
	{
		public WhereClauseProperty(string name, string associationPath)
			: base(name, associationPath)
		{
		}

		#region Expressions

		public WhereClause<T> Between(object lo, object hi)
		{
			AbstractCriterion betweenExpression = new BetweenExpression(name, lo, hi);
			AddCriterion(betweenExpression);
			return this;
		}

		public WhereClause<T> EqProperty(string otherPropertyName)
		{
			AbstractCriterion eqPropertyExpression = new EqPropertyExpression(name, otherPropertyName);
			AddCriterion(eqPropertyExpression);
			return this;
		}


		public WhereClause<T> Ge(object value)
		{
			AbstractCriterion geExpression = new GeExpression(name, value);
			AddCriterion(geExpression);
			return this;
		}

		public WhereClause<T> Gt(object value)
		{
			AbstractCriterion gtExpression = new GtExpression(name, value);
			AddCriterion(gtExpression);
			return this;
		}

		public WhereClause<T> InsensitiveLike(object value)
		{
			AbstractCriterion insensitiveLikeExpression = new InsensitiveLikeExpression(name, value);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}

		public WhereClause<T> InsensitiveLike(string value, MatchMode matchMode)
		{
			AbstractCriterion insensitiveLikeExpression = new InsensitiveLikeExpression(name, value, matchMode);
			AddCriterion(insensitiveLikeExpression);
			return this;
		}

		public WhereClause<T> Le(object value)
		{
			AbstractCriterion leExpression = new LeExpression(name, value);
			AddCriterion(leExpression);
			return this;
		}

		public WhereClause<T> LeProperty(string otherPropertyName)
		{
			AbstractCriterion lePropertyExpression = new LePropertyExpression(name, otherPropertyName);
			AddCriterion(lePropertyExpression);
			return this;
		}

		public WhereClause<T> Like(object value)
		{
			AbstractCriterion likeExpression = new LikeExpression(name, value);
			AddCriterion(likeExpression);
			return this;
		}

		public WhereClause<T> Like(string value, MatchMode matchMode)
		{
			AbstractCriterion likeExpression = new LikeExpression(name, value, matchMode);
			AddCriterion(likeExpression);
			return this;
		}

		public WhereClause<T> Lt(object value)
		{
			AbstractCriterion ltExpression = new LtExpression(name, value);
			AddCriterion(ltExpression);
			return this;
		}

		public WhereClause<T> LtProperty(string otherPropertyName)
		{
			AbstractCriterion ltPropertyExpression = new LtPropertyExpression(name, otherPropertyName);
			AddCriterion(ltPropertyExpression);
			return this;
		}

		#endregion

		#region Operator Overloading Magic

		public static WhereClause<T> operator >(WhereClauseProperty<T> expr, object other)
		{
			return expr.Gt(other);
		}

		public static WhereClause<T> operator <(WhereClauseProperty<T> expr, object other)
		{
			return expr.Lt(other);
		}

		public static WhereClause<T> operator >=(WhereClauseProperty<T> expr, object other)
		{
			return expr.Ge(other);
		}

		public static WhereClause<T> operator <=(WhereClauseProperty<T> expr, object other)
		{
			return expr.Le(other);
		}

		#endregion
	}

	#endregion

	#region OrderByEntity<T>

	public partial class OrderByEntity<T> : QueryPart
	{
		public OrderByEntity(string name, string associationPath)
			: base(name, associationPath)
		{
		}
	}

	#endregion

	#region OrderByClauseProperty<T>

	public partial class OrderByClauseProperty<T> : SimpleClause<T, OrderByClauseProperty<T>>
	{
		protected bool ascending;

		public OrderByClauseProperty(string name, string associationPath, bool backTrack)
			: this(name, associationPath, backTrack, true)
		{
		}

		public OrderByClauseProperty(string name, string associationPath)
			: this(name, associationPath, false)
		{
		}

		public OrderByClauseProperty(string name, string associationPath, bool backTrack, bool ascending)
			: base(name, associationPath, backTrack)
		{
			this.ascending = ascending;
		}

		protected override void AddCriteria(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias)
		{
			if (clauses == null)
				rootCriteria.AddOrder(this);
			else
				foreach (OrderByClauseProperty<T> o in clauses)
					o.BuildQuery(rootCriteria, criterias);
		}

		#region Expressions

		public OrderByClauseProperty<T> Asc
		{
			get
			{
				ascending = true;
				return this;
			}
		}

		public OrderByClauseProperty<T> Desc
		{
			get
			{
				ascending = false;
				return this;
			}
		}

		#endregion

		#region Operator Overloading Magic

		public static OrderByClauseProperty<T> operator &(OrderByClauseProperty<T> lhs, OrderByClauseProperty<T> rhs)
		{
			return Combine(lhs, rhs, new OrderByClauseProperty<T>(lhs.name, lhs.associationPath, lhs.backTrack, lhs.ascending));
		}

		public static bool operator true(OrderByClauseProperty<T> exp)
		{
			return false;
		}

		public static bool operator false(OrderByClauseProperty<T> exp)
		{
			return false;
		}

		public static implicit operator Order(OrderByClauseProperty<T> order)
		{
			return new Order(order.AliasAndName, order.ascending);
		}

		#endregion
	}

	#endregion

	#region ProjectionClauseProperty<T>

	public class ProjectionClauseProperty<T> : SimpleClause<T, ProjectionClauseProperty<T>>
	{
		#region Fields

		protected internal bool hasGrouping;
		protected internal bool hasDistinct;
		protected internal IProjection projection;

		#endregion

		#region ctor

		public ProjectionClauseProperty(string name, string associationPath)
			: this(name, associationPath, false)
		{
		}

		public ProjectionClauseProperty(string name, string associationPath, bool backTack)
			: base(name, associationPath, backTack)
		{
		}

		public ProjectionClauseProperty(string name, string associationPath, bool backTack, IProjection projection)
			: base(name, associationPath, backTack)
		{
			this.projection = projection;
		}

		#endregion

		#region Projections

		public ProjectionClauseProperty<T> ProjectBy()
		{
			projection = Projections.Property(AliasAndName);
			return this;
		}

		public ProjectionClauseProperty<T> GroupBy()
		{
			return MakeGroup(Projections.GroupProperty(AliasAndName));
		}

		public ProjectionClauseProperty<T> Count()
		{
			return MakeGroup(Projections.Count(AliasAndName));
		}

		public ProjectionClauseProperty<T> CountDistinct()
		{
			return MakeGroup(Projections.CountDistinct(AliasAndName));
		}

		public ProjectionClauseProperty<T> Min()
		{
			return MakeGroup(Projections.Min(AliasAndName));
		}

		public ProjectionClauseProperty<T> Max()
		{
			return MakeGroup(projection);
		}

		protected ProjectionClauseProperty<T> MakeGroup(IProjection groupProjection)
		{
			projection = groupProjection;
			hasGrouping = true;
			return this;
		}

		#endregion

		#region QueryClause<T> Overrides

		public override void BuildQuery(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias)
		{
			if (clauses != null)
			{
				foreach (ProjectionClauseProperty<T> p in clauses)
					p.EnsureCriteriaExistsForAlias(rootCriteria, criterias);
			}
			else
			{
				EnsureCriteriaExistsForAlias(rootCriteria, criterias);
			}
			rootCriteria.SetProjection((ProjectionList)this);
		}

		protected override void AddCriteria(DetachedCriteria rootCriteria, IDictionary<string, DetachedCriteria> criterias)
		{
			// Do nothing here handled in BuildQuery
		}

		#endregion

		#region Operator Overloading Magic

		public static implicit operator ProjectionList(ProjectionClauseProperty<T> expr)
		{
			if (expr.hasDistinct)
			{
				if (expr.clauses == null || expr.clauses.Count == 0)
					throw new InvalidOperationException("Cannot create distinct query with zero projections.");
				return Projections.ProjectionList().Add(Projections.Distinct((ProjectionList)expr));
			}

			if (expr.clauses == null || expr.clauses.Count == 0)
				return Projections.ProjectionList().Add(EnsureDefault(expr));

			ProjectionList list = Projections.ProjectionList();
			foreach (ProjectionClauseProperty<T> p in expr.clauses)
				list.Add(EnsureDefault(p));
			return list;
		}

		private static IProjection EnsureDefault(ProjectionClauseProperty<T> expr)
		{
			if (expr.projection == null)
				return expr.hasGrouping ? Projections.GroupProperty(expr.AliasAndName) : Projections.Property(expr.AliasAndName);
			return expr.projection;
		}

		public static ProjectionClauseProperty<T> operator &(ProjectionClauseProperty<T> lhs, ProjectionClauseProperty<T> rhs)
		{
			if (rhs.hasDistinct)
				throw new InvalidOperationException("Cannot add distinct projection to existing projections.");

			if (lhs.hasDistinct && rhs.hasGrouping)
			{
				lhs.hasGrouping = true;
				return
					Combine(ConvertToGrouping(lhs, false), rhs,
									new ProjectionClauseProperty<T>(lhs.name, lhs.associationPath, lhs.backTrack,
																									ConvertToGrouping(lhs.AliasAndName, lhs.projection)));
			}

			if (lhs.hasGrouping && !rhs.hasGrouping)
				return
					Combine(lhs, ConvertToGrouping(rhs, true),
									new ProjectionClauseProperty<T>(lhs.name, lhs.associationPath, lhs.backTrack, lhs.projection));

			if (!lhs.hasGrouping && rhs.hasGrouping)
				return
					Combine(ConvertToGrouping(lhs, false), rhs,
									new ProjectionClauseProperty<T>(lhs.name, lhs.associationPath, lhs.backTrack,
																									ConvertToGrouping(lhs.AliasAndName, lhs.projection)));
			return
				Combine(lhs, rhs, new ProjectionClauseProperty<T>(lhs.name, lhs.associationPath, lhs.backTrack, lhs.projection));
		}

		internal static IProjection ConvertToGrouping(string aliasAndName, IProjection projection)
		{
			if (projection != null && projection.IsGrouped)
				return projection;

			if (projection != null && !(projection is PropertyProjection))
				throw new InvalidOperationException(
					String.Format("Cannot auto convert {0} type to a GroupPropertyProjection.", projection.GetType()));

			return Projections.GroupProperty(aliasAndName);
		}

		private static ProjectionClauseProperty<T> ConvertToGrouping(ProjectionClauseProperty<T> expr, bool addToFront)
		{
			if (expr.clauses == null || expr.clauses.Count == 0)
				return new ProjectionClauseProperty<T>(expr.name, expr.associationPath, expr.backTrack,
																	 ConvertToGrouping(expr.AliasAndName, expr.projection));

			ProjectionClauseProperty<T> converted;
			if (addToFront)
			{
				converted = new ProjectionClauseProperty<T>(expr.Name, expr.associationPath, expr.backTrack,
																										ConvertToGrouping(expr.AliasAndName, expr.projection));
				converted.hasGrouping = true;
				converted.AddOne(converted);
			}
			else
			{
				converted = new ProjectionClauseProperty<T>(expr.clauses[0].Name, expr.clauses[0].associationPath, expr.clauses[0].backTrack,
																					ConvertToGrouping(expr.clauses[0].AliasAndName,
																														expr.clauses[0].projection));
				ProjectionClauseProperty<T> np =
					new ProjectionClauseProperty<T>(converted.Name, converted.associationPath, converted.backTrack,
																					converted.projection);
				np.hasGrouping = true;
				converted.AddOne(np);
				converted.hasGrouping = true;
				expr.clauses.RemoveAt(0);
			}

			foreach (ProjectionClauseProperty<T> p in expr.clauses)
			{
				ProjectionClauseProperty<T> np =
					new ProjectionClauseProperty<T>(p.name, p.associationPath, p.backTrack,
																					ConvertToGrouping(p.AliasAndName, p.projection));
				np.hasGrouping = true;
				p.clauses.Add(np);

			}
			return converted;
		}

		public static bool operator true(ProjectionClauseProperty<T> exp)
		{
			return false;
		}

		public static bool operator false(ProjectionClauseProperty<T> exp)
		{
			return false;
		}

		#endregion
	}

	#endregion

	#region ProjectionPropertyNumberic<T>

	public class ProjectionClausePropertyNumeric<T> : ProjectionClauseProperty<T>
	{
		public ProjectionClausePropertyNumeric(string name, string associationPath)
			: base(name, associationPath)
		{
		}

		public ProjectionClausePropertyNumeric(string name, string associationPath, bool backTrack)
			: base(name, associationPath, backTrack)
		{
		}

		public ProjectionClauseProperty<T> Sum()
		{
			return MakeGroup(Projections.Sum(AliasAndName));
		}

		public ProjectionClauseProperty<T> Avg()
		{
			return MakeGroup(Projections.Avg(AliasAndName));
		}
	}

	#endregion

	#region ProjectionRoot<T>

	public partial class ProjectionRoot<T>
	{
		public ProjectionClauseProperty<T> RowCount
		{
			get { return new ProjectionClauseProperty<T>(null, null, false, Projections.RowCount()); }
		}

		public ProjectionClauseProperty<T> SqlGroupProjection(string sql, string groupBy, string[] columnAliases,
																													IType[] types)
		{
			return
				new ProjectionClauseProperty<T>("this", null, false,
																				Projections.SqlGroupProjection(sql, groupBy, columnAliases, types));
		}

		public ProjectionClauseProperty<T> SqlProjection(string sql, string[] columnAliases, IType[] types)
		{
			return new ProjectionClauseProperty<T>("this", null, false, Projections.SqlProjection(sql, columnAliases, types));
		}
	}

	#endregion

	#region ProjectionEntity<T>

	public partial class ProjectionEntity<T> : QueryPart
	{
		public ProjectionEntity(string name, string associationPath)
			: base(name, associationPath)
		{
		}

		public ProjectionEntity(string name, string associationPath, bool backTrack) : base(name, associationPath, backTrack)
		{}
	}

	#endregion

	#region SubQuery

	public class Subquery
	{
		private DetachedCriteria subQuery;

		internal Subquery(DetachedCriteria subQuery)
		{
			this.subQuery = subQuery;
		}

		public static implicit operator DetachedCriteria(Subquery subquery)
		{
			return subquery.subQuery;
		}
	}

	#endregion
}

