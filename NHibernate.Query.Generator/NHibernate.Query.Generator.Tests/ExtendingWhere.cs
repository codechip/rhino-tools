using NHibernate;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.ActiveRecord;
using NHibernate.SqlCommand;

namespace Query
{
	public partial class Where
	{
		public partial class Root_Query_User
		{
			public virtual QueryBuilder<User> IsInGroup(string groupName)
			{
				AbstractCriterion criterion = Expression.Sql("? in (select 'Administrators')", 
															 groupName, NHibernateUtil.String);
				return FromCriterion(criterion, "this", null);
			}
		}
	}
}
