namespace MyBlog.Console
{
	using System;
	using System.Collections;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Dialect;
	using NHibernate.Dialect.Function;
	using NHibernate.Expression;
	using Query;

	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Configuration configuration = new Configuration()
					.Configure("hibernate.cfg.xml");
				ISessionFactory sessionFactory = configuration
					.BuildSessionFactory();
				//new SchemaExport(configuration).Execute(true,true,false,false);

				using (ISession session = sessionFactory.OpenSession())
				{
					DetachedCriteria blogAuthorIsJosh = Where.User.Username == "josh";
					blogAuthorIsJosh.CreateCriteria("Blogs", "userBlog")
						.SetProjection(Projections.Id())
						.Add(Property.ForName("userBlog.id").EqProperty("blog.id"));
					DetachedCriteria categoryIsNh = DetachedCriteria.For(typeof(Category), "category")
						.SetProjection(Projections.Id())
						.Add(Expression.Eq("Name", "NHibernate"))
						.Add(Property.ForName("category.id").EqProperty("postCategory.id"));
					session.CreateCriteria(typeof(Post), "post")
						.CreateAlias("Categories", "postCategory")
						.Add(Subqueries.Exists(categoryIsNh))
						.CreateAlias("Comments", "comment")
						.Add(Expression.Eq("comment.Name", "ayende"))
						.CreateAlias("Blog", "blog")
						.Add(Subqueries.Exists(blogAuthorIsJosh))
						.List();
				}
			}
			catch (Exception ex)
			{
				System.Console.WriteLine(ex);
			}
		}

		public static void Index(object o, string name)
		{
		}
	}

	public class CustomFunctionsMsSql2005Dialect : MsSql2005Dialect
	{
		public CustomFunctionsMsSql2005Dialect()
		{
			RegisterFunction("rowcount", new NoArgSQLFunction("count(*) over",
				NHibernateUtil.Int32, true));
		}
	}
}
