using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Rhino.Commons.ForTesting
{
	public class NHibernateInMemoryTestFixtureBase
	{
		protected static ISessionFactory sessionFactory;
		protected static Configuration configuration;

		/// <summary>
		/// Initialize NHibernate and builds a session factory
		/// Note, this is a costly call so it will be executed only one.
		/// </summary>
		public static void OneTimeInitalize(params Assembly [] assemblies)
		{
			if(sessionFactory!=null)
				return;
			Hashtable properties = new Hashtable();
			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;");

			configuration = new Configuration();
			configuration.Properties = properties;
			foreach (Assembly assembly in assemblies)
			{
				configuration = configuration.AddAssembly(assembly);
			}
			sessionFactory = configuration.BuildSessionFactory();
		}
		
		public ISession CreateSession()
		{
			ISession openSession = sessionFactory.OpenSession();
			IDbConnection connection = openSession.Connection;
			new SchemaExport(configuration).Execute(false,true,false,true,connection,null);
			return openSession;
		}
	}
}
