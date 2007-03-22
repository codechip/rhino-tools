using System;
using System.Collections;
using System.Data;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.MicroKernel;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Rhino.Commons.ForTesting
{
	public class ActiveRecordInMemoryTestFixtureBase
	{
		private static bool init = false;
		private IDbConnection dbConnection;

		private class InPlaceConfigurationSource_AlwaysLazy_AndPluralized  : InPlaceConfigurationSource
		{
			public InPlaceConfigurationSource_AlwaysLazy_AndPluralized()
			{
				SetIsLazyByDefault(true);
				SetPluralizeTableNames(true);
			}
		}

		/// <summary>
		/// Initialize Active Record, and initialize the container.
		/// If <paramref name="rhinoContainerConfig"/> is <see langword="null" /> or <see cref="string.Empty">string.Empty</see>
		/// a <see cref="RhinoContainer">RhinoContainer</see> will not be initialized.
		/// </summary>
		/// <param name="rhinoContainerConfig">The configuration file to initialize a <see cref="RhinoContainer">RhinoContainer</see> 
		/// or <see langword="null" />.</param>
		/// <param name="assemblies">The assemblies to load for NHibernate mapping files.</param>
		public void OneTimeInitalize(string rhinoContainerConfig, params Assembly[] assemblies)
		{
			OneTimeInitalize(rhinoContainerConfig, new InPlaceConfigurationSource_AlwaysLazy_AndPluralized(), assemblies);
		}

		/// <summary>
		/// Initialize Active Record, and initialize the container.
		/// If <paramref name="rhinoContainerConfig"/> is <see langword="null" /> or <see cref="string.Empty">string.Empty</see>
		/// a <see cref="RhinoContainer">RhinoContainer</see> will not be initialized.
		/// </summary>
		/// <param name="rhinoContainerConfig">The configuration file to initialize a <see cref="RhinoContainer">RhinoContainer</see> 
		/// or <see langword="null" />.</param>
		/// <param name="cfg">The configuration to supply to AR</param>
		/// <param name="assemblies">The assemblies to load for NHibernate mapping files.</param>
		public void OneTimeInitalize(string rhinoContainerConfig, InPlaceConfigurationSource cfg, params Assembly[] assemblies)
		{
			if (cfg == null) throw new ArgumentNullException("cfg");

			if (init)
				return;
			init = true;

			cfg.Add(typeof(ActiveRecordBase), CreateProperties());
			
			//here we either configure the IUnitOfWorkFactory appropriately (which calls ActiveRecordStarter)
			//or we configure ActiveRecordStarter ourselves

			if (!string.IsNullOrEmpty(rhinoContainerConfig))
			{
				if (!IoC.IsInitialized)
					IoC.Initialize(new RhinoContainer(rhinoContainerConfig));
				IHandler unitOfWorkFactoryHandler = IoC.Container.Kernel
					.GetHandler(typeof(IUnitOfWorkFactory));
				unitOfWorkFactoryHandler
					.AddCustomDependencyValue("configurationSource", cfg);
				unitOfWorkFactoryHandler
					.AddCustomDependencyValue("assemblies", assemblies);
			}
			else
			{
				ActiveRecordStarter.Initialize(assemblies, cfg);				
			}
		}

		protected virtual Hashtable CreateProperties()
		{
			Hashtable properties = new Hashtable();
			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
			properties.Add("hibernate.show_sql", "true");
			properties.Add("hibernate.connection.release_mode", "on_close");
			return properties;
		}

		/// <summary>
		/// Initialize NHibernate and builds a session factory
		/// Note, this is a costly call so it will be executed only one.
		/// </summary>
		public void OneTimeInitalize(params Assembly[] assemblies)
		{
			OneTimeInitalize(null, assemblies);
		}

		/// <summary>
		/// Creates the in memory db schema using the scope
		/// </summary>
		public void SetupDB()
		{
			ISessionFactoryHolder sessionFactoryHolder = ActiveRecordMediator.GetSessionFactoryHolder();
			if(sessionFactoryHolder.ThreadScopeInfo.HasInitializedScope==false)
			{
				throw new InvalidOperationException("Must be inside a scope for InMemory tests to work");
			}
			Configuration configuration = sessionFactoryHolder.GetConfiguration(typeof(ActiveRecordBase));
			ISession session = sessionFactoryHolder.CreateSession(typeof(ActiveRecordBase));
			try
			{
				new SchemaExport(configuration).Execute(false, true, false, true, session.Connection, null);
			}
			finally
			{
				sessionFactoryHolder.ReleaseSession(session);
			}
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
		    ISession session = ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof (ActiveRecordBase));
			dbConnection = session.Connection;
			SetupDB();
		}

		public IUnitOfWork CreateNestedUnitOfWork()
		{
			if (dbConnection==null)
			{
				throw new InvalidOperationException(
					"Did you forgot to call CreateUnitOfWork()? ActiveRecordInMemoryTextFixtureBase did not create any previous unit of work.");
			}
			return UnitOfWork.Start(dbConnection, UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork);
		}

		public void DisposeUnitOfWork()
		{
			UnitOfWork.Current.Dispose();
			dbConnection.Dispose();
			dbConnection = null;
		}
	}
}
