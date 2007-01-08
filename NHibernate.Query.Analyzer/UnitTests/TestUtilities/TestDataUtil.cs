#region Using directives

using System;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Model;
using NHibernate.Cfg;
using NHibernate.Driver;

#endregion

namespace Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities
{
	public class TestDataUtil
	{

		public static IProjectsRepository CreateFileRepository()
		{
			string constr = "Data Source={0};New=True;UTF8Encoding=True;Version=3";
			string tmpFile = Path.GetTempFileName();
			constr = string.Format(constr, tmpFile);
			File.Delete(tmpFile); //GetTempFileName create a zero size file
			Configuration cfg = new Configuration();
			cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			cfg.SetProperty("hibernate.connection.driver_class", typeof(SQLite20Driver).AssemblyQualifiedName);
			cfg.SetProperty("hibernate.connection.connection_string", constr);
			//cfg.SetProperty("hibernate.connection.release_mode", "on_close");
			cfg.AddAssembly(typeof (IProjectsRepository).Assembly);
			return new ProjectsRepository(cfg);
		}

		public static string TestConfigFile
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll.config"); }
		}

		public static string TestDllFile
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll"); }
		}

		public static string TestMappingFile
		{
			get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.hbm.xml"); }
		}

		public const string HibernateConnectionString = "hibernate.connection.connection_string";
	}
}