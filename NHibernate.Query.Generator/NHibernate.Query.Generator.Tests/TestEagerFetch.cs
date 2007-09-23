using System;
using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.WithEagerFetch;
using NHibernate.SqlCommand;
using Query;

namespace NHibernate.Query.Generator.Tests
{
    [TestFixture]
    public class TestApplicationRepositoryFind
    {
        private Configuration cfg;
        private ISessionFactory sf;
        private ISession session;

        private Application app1;
        private Application app2;
        private Application app3;
        private Operation app3op1 = new Operation();
        private Operation app3op2 = new Operation();



        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            cfg = new Configuration()
                .SetProperty("hibernate.show_sql", "true")
                .SetProperty("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect")
                .SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver")
                .SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider")
                .SetProperty("hibernate.connection.connection_string", "Data Source=test.db3;Version=3;New=True;")
//                .SetProperty("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;")
                .SetProperty("hibernate.connection.release_mode", "on_close")
                .SetProperty("hibernate.max_fetch_deptch", "2")
				.AddClass(typeof(WithEagerFetch.Application))
				.AddClass(typeof(WithEagerFetch.Action));

            sf = cfg.BuildSessionFactory();
        }



        [SetUp]
        public void SetUpEachTest()
        {
            new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Execute(false, true, false, false);

            app1 = new Application();
            app1.Name = "Mega App";
            app1.Description = "Fabulous";
            app1.Obsolete = false;

            app2 = new Application();
            app2.Name = "Super Mega App";
            app2.Description = "Fabulous";
            app2.Obsolete = false;

            app3 = new Application();
            app3.Name = "Ultra Super Mega App";
            app3.Description = "Amazing";
            app3.Obsolete = false;

            app3op1.Application = app3;
            app3op1.Name = "EatCheese";
            app3op1.Description = "Allows the user eat cheese";
            app3.Operations.Add(app3op1);

            app3op2.Application = app3;
            app3op2.Name = "MakeTea";
            app3op2.Description = "Allows the user make tea";
            app3.Operations.Add(app3op2);

            session = sf.OpenSession();
            ITransaction tx = session.BeginTransaction();

            session.Save(app1);
            session.Save(app2);
            session.Save(app3);
            session.Save(app3op1);
            session.Save(app3op2);

            tx.Commit();
            session.Close();
        }



        [TearDown]
        public void TearDownEachTest()
        {
            if (session.IsOpen) session.Close();
        }



        [Test]
        public void ShouldFetchJoinWithoutCriteria()
        {
            session = sf.OpenSession();

            DetachedCriteria where =
                Where.Application.Operations.With(JoinType.LeftOuterJoin, FetchMode.Join);

            IList<Application> applications =
                where.GetExecutableCriteria(session).SetResultTransformer(CriteriaUtil.DistinctRootEntity).
                    List<Application>();

            session.Close();

            Assert.IsNotNull(applications);
            Assert.AreEqual(3, applications.Count);
            Assert.IsTrue(applications.Contains(app1));
            Assert.IsTrue(applications.Contains(app2));
            Assert.IsTrue(applications.Contains(app3));
            Assert.AreEqual(2, applications[2].Operations.Count);
            Assert.IsTrue(applications[2].Operations.Contains(app3op1));
            Assert.IsTrue(applications[2].Operations.Contains(app3op2));
        }



        [Test]
        public void ShouldFetchJoinWithCriteria()
        {
            session = sf.OpenSession();

            DetachedCriteria where =
                Where.Application.Operations
                    .With(JoinType.LeftOuterJoin, FetchMode.Join).Name == "EatCheese";

            IList<Application> applications = 
                where.GetExecutableCriteria(session).SetResultTransformer(CriteriaUtil.DistinctRootEntity)
                    .List<Application>();

            session.Close();

            Assert.IsNotNull(applications);
            Assert.AreEqual(1, applications.Count);
            Assert.AreEqual(app3, applications[0]);
            Assert.AreEqual(1, applications[0].Operations.Count);
            Assert.IsTrue(applications[0].Operations.Contains(app3op1));
        }



        [Test]
        public void ShouldFetchJoinWithCriteriaOnRootAndJoin()
        {
            session = sf.OpenSession();

            DetachedCriteria where =
                Where.Application.Id == app3.Id &
                Where.Application.Operations
                    .With(JoinType.LeftOuterJoin, FetchMode.Join).Name == "EatCheese";

            IList<Application> applications =
                where.GetExecutableCriteria(session).SetResultTransformer(CriteriaUtil.DistinctRootEntity)
                    .List<Application>();

            session.Close();

            Assert.IsNotNull(applications);
            Assert.AreEqual(1, applications.Count);
            Assert.AreEqual(app3, applications[0]);
            Assert.AreEqual(1, applications[0].Operations.Count);
            Assert.IsTrue(applications[0].Operations.Contains(app3op1));
        }
    }
}