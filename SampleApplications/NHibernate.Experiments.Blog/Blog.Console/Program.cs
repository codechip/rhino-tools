using System;
using System.Collections;
using System.IO;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace MyBlog.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("nhprof.log4net.config"));
            try
            {
                Configuration configuration = new Configuration()
                    .Configure("hibernate.cfg.xml");
                ISessionFactory sessionFactory = configuration
                    .BuildSessionFactory();
                //new SchemaExport(configuration).Execute(true,true,false,false);

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction tx = session.BeginTransaction())
                {
                    var blog = session.Get<Blog>(1);
                    foreach (Post post in blog.Posts)
                    {
                        System.Console.WriteLine(post.Title);
                    }
                    tx.Commit();
                }

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction tx = session.BeginTransaction())
                {
                    IList enumerable = session.CreateQuery("from Blog b join fetch b.Posts")
                        .SetMaxResults(5)
                        .SetCacheable(true)
                        .List();
                    tx.Commit();
                }

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction tx = session.BeginTransaction())
                {
                    var post = session.CreateCriteria(typeof (Post))
                        .SetMaxResults(1)
                        .UniqueResult<Post>();
                    post.Title = "123";
                    tx.Commit();
                }

                using (ISession session = sessionFactory.OpenSession())
                using (ITransaction tx = session.BeginTransaction())
                {
                    IList enumerable = session.CreateQuery("from Blog b join fetch b.Posts")
                        .SetMaxResults(5)
                        .SetCacheable(true)
                        .List();

                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine((object) ex);
            }
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