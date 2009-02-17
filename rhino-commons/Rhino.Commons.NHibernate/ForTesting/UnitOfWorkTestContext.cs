#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Castle.Windsor;
using NHibernate;
using NHibernate.Cfg;
using Environment=NHibernate.Cfg.Environment;

namespace Rhino.Commons.ForTesting
{
    public enum DatabaseEngine
    {
        SQLite,
        MsSqlCe,
        MsSql2005,
        MsSql2005Express
    }

    public enum PersistenceFramework
    {
        ActiveRecord,
        NHibernate
    }

    /// <summary>
    /// Encapsulates the construction, initialization and disposal of the
    /// database and UnitOfWork context required for testing. Allows different
    /// implementations of this context to be substituted at runtime
    /// </summary>
    public abstract class UnitOfWorkTestContext  : IDisposable
    {
        private const string activeRecordTestContextType = "Rhino.Commons.ForTesting.ARUnitOfWorkTestContext, Rhino.Commons.ActiveRecord";
        private static Type activeRecordType;

        public static UnitOfWorkTestContext For(PersistenceFramework framwork,
                                                string rhinoContainerConfig,
                                                UnitOfWorkTestContextDbStrategy dbStrategy,
                                                MappingInfo mappingInfo)
        {
            switch (framwork)
            {
                case PersistenceFramework.ActiveRecord:
                    return CreateActiveRecordImplementation(dbStrategy, mappingInfo, rhinoContainerConfig);
                case PersistenceFramework.NHibernate:
                    return new NHibernateUnitOfWorkTestContext(dbStrategy, rhinoContainerConfig, mappingInfo);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

		public static UnitOfWorkTestContext For(PersistenceFramework framwork,
                                                IWindsorContainer container,
                                                UnitOfWorkTestContextDbStrategy dbStrategy,
                                                MappingInfo mappingInfo)
		{
			UnitOfWorkTestContext context = For(framwork, string.Empty, dbStrategy, mappingInfo);
			context.rhinoContainer = container;
			return context;
		}


        private static UnitOfWorkTestContext CreateActiveRecordImplementation(UnitOfWorkTestContextDbStrategy dbStrategy,
                                                                                MappingInfo mappingInfo,
                                                                                string rhinoContainerConfig)
        {
            if (activeRecordType == null)
                activeRecordType = Type.GetType(activeRecordTestContextType);
			if (activeRecordType == null)
			{
				throw new InvalidOperationException("Could not find Active Record context type. Did you forget to reference 'Rhino.Commons.ActiveRecord'?");
			}
            return
                (UnitOfWorkTestContext)
                Activator.CreateInstance(activeRecordType,
                                         new object[] {dbStrategy, rhinoContainerConfig, mappingInfo});
        }


    	private readonly string rhinoContainerConfigPath;
        private readonly UnitOfWorkTestContextDbStrategy dbStrategy;
        private readonly MappingInfo mappingInfo;
        private IWindsorContainer rhinoContainer;
        private int unitOfWorkNestingLevel = -1;


        public UnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                     string rhinoContainerConfigPath,
                                     MappingInfo assemblies)
        {
            this.rhinoContainerConfigPath = rhinoContainerConfigPath ?? string.Empty;
            this.dbStrategy = dbStrategy;
            mappingInfo = assemblies;
            this.dbStrategy.TestContext = this;
        }
		

        protected MappingInfo MappingInfo
        {
            get { return mappingInfo; }
        }

        public abstract Configuration Configuration
        {
            get;
        }

        public string DatabaseName
        {
            get { return dbStrategy.DatabaseName; }
        }

        public DatabaseEngine DatabaseEngine
        {
            get { return dbStrategy.DatabaseEngine; }
        }

        protected UnitOfWorkTestContextDbStrategy DbStrategy
        {
            get { return dbStrategy; }
        }

        public abstract PersistenceFramework Framework { get; }

        public IWindsorContainer RhinoContainer
        {
            get
            {
                if (string.IsNullOrEmpty(rhinoContainerConfigPath) && rhinoContainer==null)
                    return null;
                return 
                    rhinoContainer =
                    rhinoContainer ?? new RhinoContainer(rhinoContainerConfigPath);
            }
        }

        public string RhinoContainerConfigPath
        {
            get { return rhinoContainerConfigPath; }
        }

        public abstract ISessionFactory SessionFactory { get; }


        public int UnitOfWorkNestingLevel
        {
            get { return unitOfWorkNestingLevel; }
        }


        /// <summary>
        /// Starts a <see cref="UnitOfWork" /> and creates the db schema.
        /// </summary>
        /// <example>Using <see cref="Commons.RhinoContainer" />, <see cref="IoC" />, and <see cref="UnitOfWork" /> in your tests.
        /// <code lang="c#" escaped="true">
        /// using MbUnit.Framework;
        /// using Rhino.Commons;
        /// using Rhino.Commons.ForTesting;
        /// 
        /// [TestFixture]
        /// public class FooTest : DatabaseTestFixtureBase
        /// {
        ///		[TestFixtureSetup]
        ///		public void TestFixtureSetup()
        ///		{
        ///			InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, "RhinoContainer.boo", typeof(Foo).Assembly);
        ///		}
        /// 
        ///		[Setup]
        ///		public void TestSetup()
        ///		{
        ///			/// Creates a top level UnitOfWork, remember to clean me up
        ///			CurrentContext.CreateUnitOfWork();
        ///		}
        /// 
        ///		[TearDown]
        ///		public void TestTearDown()
        ///		{
        ///			/// Cleanup the top level UnitOfWork
        ///			CurrentContext.DisposeUnitOfWork();
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
        ///				IoC.Resolve&lt;IRepository&lt;Foo&gt;&gt;().Save(f);
        ///			});
        ///			
        ///			Assert.AreNotEqual(Guid.Empty, f.Id);
        /// 
        ///			using(UnitOfWork.Start())
        ///				res = IoC.Resolve&lt;IRepository&lt;Foo&gt;&gt;().Load(f.Id);
        /// 
        ///			Assert.IsNotNull(res);
        ///			Assert.AreEqual("Bar", res.Name);
        ///		}
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="Commons.RhinoContainer" />
        /// <seealso cref="IoC" />
        /// <seealso cref="UnitOfWork" />
        public virtual void CreateUnitOfWork()
        {
            if (unitOfWorkNestingLevel == 0)
            {
                throw new InvalidOperationException(
                    "Cannot create a nested UnitOfWork with this method. Use CreateNestedUnitOfWork() instead");
            }

            UnitOfWork.Start();
            SetupDatabase(UnitOfWork.CurrentSession);
            unitOfWorkNestingLevel = 0;
        }


        public IUnitOfWork CreateNestedUnitOfWork()
        {
            if (unitOfWorkNestingLevel == -1)
            {
                throw new InvalidOperationException(
                    "Did you forgot to call CreateUnitOfWork()? UnitOfWorkTestContext did not create any previous unit of work.");
            }
            IUnitOfWork uow =
                UnitOfWork.Start(UnitOfWork.CurrentSession.Connection, UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork);
            unitOfWorkNestingLevel += 1;
            return uow;
        }


        /// <summary>
        /// Opens an NHibernate session and creates the db schema.
        /// </summary>
        /// <returns>The open NHibernate session.</returns>
        public virtual ISession CreateSession()
        {
            ISession session = dbStrategy.CreateSession();
            SetupDatabase(session);
            return session;
        }


        public void DisposeSession(ISession sessionToClose)
        {
            //explicit disposing of connection is required (??) for in memory databases
            IDbConnection con = sessionToClose.Connection;
            sessionToClose.Dispose();
            con.Dispose();
        }


        public virtual void DisposeUnitOfWork()
        {
            UnitOfWork.Current.Dispose();
            unitOfWorkNestingLevel -= 1;
        }

        public abstract void InitializeContainerAndUowFactory();


        /// <summary>
        /// Creates the in db schema using the session.
        /// </summary>
        /// <param name="session">An open NHibernate session.</param>
        public void SetupDatabase(ISession session)
        {
            dbStrategy.SetupDatabase(session);
        }


        public override string ToString()
        {
            return
                string.Format("Framework: {0}; DatabaseEngine: {1}; DatabaseName: {2}; RhinoContainerPath: {3}",
                              Framework,
                              DatabaseEngine,
                              DatabaseName,
                              RhinoContainerConfigPath ?? "");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (rhinoContainer != null) rhinoContainer.Dispose();
        }

        #endregion

        private class NHibernateUnitOfWorkTestContext : UnitOfWorkTestContext
        {
            private Configuration configs;
            private ISessionFactory sessionFactory;


            public NHibernateUnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                                   string rhinoContainerConfigPath,
                                                   MappingInfo assemblies)
                : base(dbStrategy, rhinoContainerConfigPath, assemblies) {}
			

            public override Configuration Configuration
            {
                get { return configs = configs ?? CreateConfigs(); }
            }

            public override PersistenceFramework Framework
            {
                get { return PersistenceFramework.NHibernate; }
            }

            public override ISessionFactory SessionFactory
            {
                get { return sessionFactory = sessionFactory ?? BuildSessionFactory(); }
            }

            private ISessionFactory BuildSessionFactory()
            {
                //TODO: This is hack for rev 1705. there should be a better way
                if (Configuration.GetProperty(Environment.ProxyFactoryFactoryClass) == null)
                    Configuration.Properties[Environment.ProxyFactoryFactoryClass] = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";
            	  ISessionFactory sessionFactory = Configuration.BuildSessionFactory();

                foreach (INHibernateInitializationAware initializer in GetNHibernateInitializers())
                    initializer.Initialized(configs, sessionFactory);

                return sessionFactory;
            }

            private Configuration CreateConfigs()
            {

				foreach (INHibernateInitializationAware initializer in GetNHibernateInitializers())
					initializer.BeforeInitialization();

				Configuration cfg = new Configuration();
                cfg.Properties = DbStrategy.NHibernateProperties;

                foreach (KeyValuePair<string, string> import in MappingInfo.QueryLanguageImports)
                    cfg.Imports[import.Key] = import.Value;

                foreach (Assembly assembly in MappingInfo.MappingAssemblies)
                    cfg.AddAssembly(assembly);

                foreach (INHibernateInitializationAware initializer in GetNHibernateInitializers())
                    initializer.Configured(cfg);

                return cfg;
            }

            private IEnumerable<INHibernateInitializationAware> GetNHibernateInitializers()
            {
                INHibernateInitializationAware[] initializers = new INHibernateInitializationAware[]{};
                if (MappingInfo.NHInitializationAware != null)
                {
                    // locally configured initializer takes precendence over a container component
                    initializers = new INHibernateInitializationAware[] {MappingInfo.NHInitializationAware};
                }
                else if (IoC.IsInitialized && IoC.Container.Kernel.HasComponent(typeof(INHibernateInitializationAware)))
                {
                    initializers = IoC.ResolveAll<INHibernateInitializationAware>();
                }

                if (initializers.Length == 0)
                    yield break;

                foreach (INHibernateInitializationAware initializer in initializers)
                    yield return initializer;
            }

            public override void InitializeContainerAndUowFactory()
            {
                ResetRhinoContainer();

                if (RhinoContainer != null)
                {
                    IoC.Initialize(RhinoContainer);
                    NHibernateUnitOfWorkFactory hibernateUnitOfWorkFactory =
                        (NHibernateUnitOfWorkFactory)IoC.Resolve<IUnitOfWorkFactory>();
                    hibernateUnitOfWorkFactory.RegisterSessionFactory(SessionFactory);
                }
            }

            protected void ResetRhinoContainer()
            {
				if (!string.IsNullOrEmpty(rhinoContainerConfigPath))
				{
					if (IoC.IsInitialized)
						IoC.Reset();
				
					rhinoContainer = null;
				}
				else
				{
					IoC.Reset(rhinoContainer);
				}
            }
        }
    }
}
