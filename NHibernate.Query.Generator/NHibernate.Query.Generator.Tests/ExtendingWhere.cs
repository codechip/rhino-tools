using NHibernate;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.ActiveRecord;
using NHibernate.SqlCommand;

namespace Query
{
	public partial class Where
	{
		public partial class Query_User<T1>
		{
			public virtual QueryBuilder<User> IsInGroup(string groupName)
			{
				QueryBuilder<User> queryBuilder = new QueryBuilder<User>("this", null);
				AbstractCriterion sql = Expression.Sql(new SqlString("? in (select 'Administrators')"), 
				                                       groupName, NHibernateUtil.String);
				queryBuilder.AddCriterion(sql);
				return queryBuilder;
			}
		}
	}
}
