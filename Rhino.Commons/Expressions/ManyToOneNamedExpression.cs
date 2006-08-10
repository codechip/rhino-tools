using NHibernate.Expression;

namespace Query
{
	public class ManyToOneNamedExpression
	{
		string name;

		public ManyToOneNamedExpression(string name)
		{
			this.name = name;
		}

		public SimpleExpression Eq(object value)
		{
			return Expression.Eq(name, value);
		}
		
		public AbstractCriterion IdIs(object value)
		{
			return Expression.Eq(string.Format("{0}.id", name), value);
		}
	}
}