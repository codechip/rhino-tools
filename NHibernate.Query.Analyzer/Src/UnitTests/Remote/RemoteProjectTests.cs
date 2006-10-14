using System;
using System.Collections;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Remote
{
	[TestFixture]
	public class RemoteProjectTests
	{
		private RemoteProject current;
		private string hqlQuery = "from TestProject t order by t.Id";
        private string expectedSql = "select testprojec0_._id as id, testprojec0_.Data as Data from TestFile testprojec0_ order by  testprojec0_._id";
		private string asmFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll");
		private string conStr = string.Format("Data Source={0};New=False;UTF8Encoding=True;Version=2", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\TestProject.db"));

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			string[] asms = {asmFilename}, 
			         mappings = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.hbm.xml")};
			current = new RemoteProject();
			current.Cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			current.Cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			current.Cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SQLiteDriver");
			current.Cfg.SetProperty("hibernate.connection.connection_string", conStr);
			current.BuildInternalProject(asms, mappings, new string[0], new ArrayList());
		}
	    
		[Test]
		public void HqlToSql()
		{
			string[] sql = current.HqlToSql(hqlQuery, new Hashtable());
			Assert.AreEqual(expectedSql, sql[0], "HQL to SQL translation failed!");
		}

		[Test]
		public void RunHql()
		{
			HqlResultGraph hrg = current.RunHql(hqlQuery);
			Assert.AreEqual(2003, hrg.Graph.Count, "Bad number of results from test database");
			object tp = hrg.Graph[0];
			Assert.AreEqual(1, ReflectionUtil.GetPropertyValue(tp,"Id"));
			Assert.AreEqual("Testing", ReflectionUtil.GetPropertyValue(tp,"Data"));
		}

		[Test]
		public void BuiltProject()
		{
			Assert.IsNotNull(Type.GetType("TestProject.TestProject, Ayende.NHibernateQueryAnalyzer.TestProject", true), "Unable to load type from dynamically loaded assembly");
			Assert.IsNotNull(current.Factory, "Factory was not built");
		}
	}
}