using System;
using System.Collections;
using System.Data;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons.Helpers;

namespace Rhino.Commons.ForTesting
{
	public class NHibernateEmbeddedDBTestFixtureBase
	{
		public static string DatabaseFilename = "TempDB.sdf";

		protected static ISessionFactory sessionFactory;
		protected static Configuration configuration;

		/// <summary>
		/// Initialize NHibernate, build a session factory, and initialize the container.
		/// If <paramref name="rhinoContainerConfig"/> is <see langword="null" /> or <see cref="string.Empty">string.Empty</see>
		/// a <see cref="RhinoContainer">RhinoContainer</see> will not be initialized.
		/// </summary>
		/// <param name="rhinoContainerConfig">The configuration file to initialize a <see cref="RhinoContainer">RhinoContainer</see> 
		/// or <see langword="null" />.</param>
		/// <param name="assemblies">The assemblies to load for NHibernate mapping files.</param>
		public static void FixtureInitialize(string rhinoContainerConfig, params Assembly[] assemblies)
		{
			if (sessionFactory != null)
				return;
			Hashtable properties = new Hashtable();
			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSqlCeDialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			string connectionString = string.Format("Data Source={0};", DatabaseFilename);
			properties.Add("hibernate.connection.connection_string", connectionString);
			properties.Add("hibernate.show_sql", "true");
			properties.Add("hibernate.connection.release_mode", "on_close");

			configuration = new Configuration();
			configuration.Properties = properties;
			foreach (Assembly assembly in assemblies)
			{
				configuration = configuration.AddAssembly(assembly);
			}
			sessionFactory = configuration.BuildSessionFactory();

			if (!string.IsNullOrEmpty(rhinoContainerConfig))
			{
				if (!IoC.IsInitialized)
					IoC.Initialize(new RhinoContainer(rhinoContainerConfig));
				NHibernateUnitOfWorkFactory.RegisterSessionFactory(sessionFactory);
			}
		}

		/// <summary>
		/// Initialize NHibernate and builds a session factory
		/// Note, this is a costly call so it will be executed only one.
		/// </summary>
		public static void FixtureInitialize(params Assembly[] assemblies)
		{
			FixtureInitialize(null, assemblies);
		}

		/// <summary>
		/// Creates the in memory db schema using the session.
		/// </summary>
		public void SetupDB()
		{
			SqlCEDbHelper.CreateDatabaseFile(DatabaseFilename);
			new SchemaExport(configuration).Execute(false, true,false,true);
		}

		/// <summary>
		/// Starts a <see cref="UnitOfWork" /> and creates the in memory db schema.
		/// </summary>
		/// <example>Using <see cref="RhinoContainer" />, <see cref="IoC" />, and <see cref="UnitOfWork" /> in your tests.
		/// <code lang="c#" escaped="true">
		/// using NUnit.Framework;
		/// using Rhino.Commons;
		/// using Rhino.Commons.ForTesting;
		/// 
		/// [TestFixture]
		/// public class FooTest : NHibernateEmbeddedDBTestFixtureBase
		/// {
		///		[TestFixtureSetup]
		///		public void TestFixtureSetup()
		///		{
		///			OneTimeInitialize("RhinoContainer.boo", typeof(Foo).Assembly);
		///		}
		/// 
		///		[Setup]
		///		public void TestSetup()
		///		{
		///			/// Creates a top level UnitOfWork, remember to clean me up
		///			CreateUnitOfWork();
		///		}
		/// 
		///		[TearDown]
		///		public void TestTearDown()
		///		{
		///			/// Cleanup the top level UnitOfWork
		///			UnitOfWork.Current.Dispose();
		///		}
		/// 
		///		[Test]
		///		public void CanSaveFoo()
		///		{
		///			Foo f = new Foo();
		///			Foo res = null;
		///			f.Name = "Bar";
		/// 
		///			Assert.AreEqual(Guid.Empty, f.Id);
		/// 
		///			With.Transaction(delegate
		///			{
		///				IoC.Resolve<IRepository<Foo>>.Save(f);
		///			});
		///			
		///			Assert.AreNotEqual(Guid.Empty, f.Id);
		/// 
		///			using(UnitOfWork.Start())
		///				res = IoC.Resolve<IRepository<Foo>>.Load(f.Id);
		/// 
		///			Assert.IsNotNull(res);
		///			Assert.AreEqual("Bar", res.Name);
		///		}
		/// }
		/// </code>
		/// </example>
		/// <seealso cref="RhinoContainer" />
		/// <seealso cref="IoC" />
		/// <seealso cref="UnitOfWork" />
		public void CreateUnitOfWork()
		{
			SetupDB();
			UnitOfWork.Start();
		}

		/// <summary>
		/// Opens an NHibernate session and creates the db schema.
		/// </summary>
		/// <returns>The open NHibernate session.</returns>
		public ISession CreateSession()
		{
			SetupDB();

			return sessionFactory.OpenSession();
		}

		public void DisposeSession(ISession sessionToClose)
		{
			sessionToClose.Dispose();
		}
	}
}
