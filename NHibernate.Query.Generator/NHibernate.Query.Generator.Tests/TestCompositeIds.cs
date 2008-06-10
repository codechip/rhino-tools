using System;
using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Query.Generator.Tests.CompositeIds;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Query;

namespace NHibernate.Query.Generator.Tests
{
	[TestFixture]
	public class TestCompositeIds
	{
		private Configuration cfg;
		private ISessionFactory sf;
		private ISession session;

		private CompositeIdParent parent;
		private CompositeIdChild child1;
		private CompositeIdChild child2;

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			cfg = new Configuration()
				.SetProperty("show_sql", "true")
				.SetProperty("dialect", "NHibernate.Dialect.SQLiteDialect")
				.SetProperty("connection.driver_class", "NHibernate.Driver.SQLite20Driver")
				.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider")
				.SetProperty("connection.connection_string", "Data Source=test.db3;Version=3;New=True;")
				//                .SetProperty("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;")
				.SetProperty("connection.release_mode", "on_close")
				.SetProperty("max_fetch_deptch", "2")
				.AddClass(typeof(CompositeIdParent))
				.AddClass(typeof(CompositeIdChild));

			sf = cfg.BuildSessionFactory();
		}

		[SetUp]
		public void SetUpEachTest()
		{
			new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Execute(false, true, false, false);

			parent = new CompositeIdParent();
			
			child1 = new CompositeIdChild();
			child1.Parent = parent;
			child1.CompositeIdPart1 = "child1key1";
			child1.CompositeIdPart2 = "child1key2";

			child2 = new CompositeIdChild();
			child1.Parent = parent;
			child2.CompositeIdPart1 = "child2key1";
			child2.CompositeIdPart2 = "child2key2";

			session = sf.OpenSession();
			ITransaction tx = session.BeginTransaction();

			session.Save(parent);
			session.Save(child1);
			session.Save(child2);

			tx.Commit();
			session.Close();
		}

		[Test]
		public void ShouldSelectIdColumnsWithExists()
		{
			session = sf.OpenSession();

			DetachedCriteria where =
				Where.CompositeIdParent.Children.Exists(Where.CompositeIdChild.CompositeIdPart1.Like("child"));

			IList<CompositeIdParent> applications =
				where.GetExecutableCriteria(session).SetResultTransformer(new DistinctRootEntityResultTransformer()).
					List<CompositeIdParent>();

			session.Close();
		}

		[TearDown]
		public void TearDownEachTest()
		{
			if(session.IsOpen)
				session.Close();
		}
	}
}
