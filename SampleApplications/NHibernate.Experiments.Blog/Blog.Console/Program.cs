using System;
using System.Collections;
using System.Collections.Generic;
using MyBlog;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Expression;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Type;
using Query;

namespace MyBlog.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			Configuration configuration = new Configuration()
				.Configure("hibernate.cfg.xml");
			ISessionFactory sessionFactory = configuration
				.BuildSessionFactory();
			//new SchemaExport(configuration).Execute(true,true,false,false);
			System.Console.Clear();
			using (ISession session = sessionFactory.OpenSession())
			{
				IList list = session.CreateQuery("select b, rowcount() from Blog b")
						.SetFirstResult(5)
						.SetMaxResults(10)
						.List();
				foreach (object[] tuple in list)
				{
					System.Console.WriteLine("Entity: {0}", ((Blog)tuple[0]).Id);
					System.Console.WriteLine("Row Count: {0}", (int)tuple[1]);
				}
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
