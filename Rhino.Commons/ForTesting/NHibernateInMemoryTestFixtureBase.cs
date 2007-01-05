using System.Collections;
using System.Data;
using System.Reflection;
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
		/// Initialize NHibernate, build a session factory, and initialize the container.
		/// If <paramref name="rhinoContainerConfig"/> is <see langword="null" /> or <see cref="string.Empty">string.Empty</see>
		/// a <see cref="RhinoContainer">RhinoContainer</see> will not be initialized.
		/// </summary>
		/// <param name="rhinoContainerConfig">The configuration file to initialize a <see cref="RhinoContainer">RhinoContainer</see> 
		/// or <see langword="null" />.</param>
		/// <param name="assemblies">The assemblies to load for NHibernate mapping files.</param>
		public static void OneTimeInitalize(string rhinoContainerConfig, params Assembly[] assemblies)
		{
			if (sessionFactory != null)
				return;
			Hashtable properties = new Hashtable();
			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
			properties.Add("hibernate.show_sql", "true");

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
		public static void OneTimeInitalize(params Assembly[] assemblies)
		{
			OneTimeInitalize(null, assemblies);
		}

		/// <summary>
		/// Creates the in memory db schema using the session.
		/// </summary>
		/// <param name="session">An open NHibernate session.</param>
		public void SetupDB(ISession session)
		{
			IDbConnection connection = session.Connection;
			new SchemaExport(configuration).Execute(false, true, false, true, connection, null);
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
		/// public class FooTest : NHibernateInMemoryTest
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
			UnitOfWork.Start();
			SetupDB(NHibernateUnitOfWorkFactory.CurrentNHibernateSession);
		}

		/// <summary>
		/// Opens an NHibernate session and creates the in memory db schema.
		/// </summary>
		/// <returns>The open NHibernate session.</returns>
		public ISession CreateSession()
		{
			//need to get our own connection, because NH will try to close it
			//as soon as possible, and we will lose the changes.
			IDbConnection dbConnection = sessionFactory.ConnectionProvider.GetConnection();
			ISession openSession = sessionFactory.OpenSession(dbConnection);
			SetupDB(openSession);
			return openSession;
		}

		public void DisposeSession(ISession sessionToClose)
		{
			IDbConnection con = sessionToClose.Connection;
			sessionToClose.Dispose();
			con.Dispose();
		}
	}
}
