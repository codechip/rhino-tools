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
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.MicroKernel;
using Castle.Windsor;
using NHibernate;
using NHibernate.Cfg;

namespace Rhino.Commons.ForTesting
{
    public enum DatabaseEngine
    {
        SQLite,
        MsSqlCe,
        MsSql2005
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
    public abstract class UnitOfWorkTestContext 
    {
        public static UnitOfWorkTestContext For(PersistenceFramework framwork,
                                                string rhinoContainerConfig,
                                                UnitOfWorkTestContextDbStrategy dbStrategy,
                                                MappingInfo mappingInfo)
        {
            switch (framwork)
            {
                case PersistenceFramework.ActiveRecord:
                    return new ARUnitOfWorkTestContext(dbStrategy, rhinoContainerConfig, mappingInfo);
                case PersistenceFramework.NHibernate:
                    return new NHibernateUnitOfWorkTestContext(dbStrategy, rhinoContainerConfig, mappingInfo);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        internal abstract Configuration Configuration
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
                if (string.IsNullOrEmpty(rhinoContainerConfigPath))
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
        /// public class FooTest : TestFixtureBase
        /// {
        ///		[TestFixtureSetup]
        ///		public void TestFixtureSetup()
        ///		{
        ///			FixtureInitialize(PersistenceFramework.NHibernate, "RhinoContainer.boo", typeof(Foo).Assembly);
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
        ///				IoC.Resolve<IRepository<Foo>>().Save(f);
        ///			});
        ///			
        ///			Assert.AreNotEqual(Guid.Empty, f.Id);
        /// 
        ///			using(UnitOfWork.Start())
        ///				res = IoC.Resolve<IRepository<Foo>>().Load(f.Id);
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

        public abstract void IntialiseContainerAndUowFactory();


        /// <summary>
        /// Creates the in db schema using the session.
        /// </summary>
        /// <param name="session">An open NHibernate session.</param>
        public void SetupDatabase(ISession session)
        {
            dbStrategy.SetupDatabase(session);
        }



        private class NHibernateUnitOfWorkTestContext : UnitOfWorkTestContext
        {
            private Configuration configs;
            private ISessionFactory sessionFactory;


            public NHibernateUnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                                   string rhinoContainerConfigPath,
                                                   MappingInfo assemblies)
                : base(dbStrategy, rhinoContainerConfigPath, assemblies) {}


            internal override Configuration Configuration
            {
                get { return configs = configs ?? CreateConfigs(); }
            }

            public override PersistenceFramework Framework
            {
                get { return PersistenceFramework.NHibernate; }
            }

            public override ISessionFactory SessionFactory
            {
                get { return sessionFactory = sessionFactory ?? Configuration.BuildSessionFactory(); }
            }


            private Configuration CreateConfigs()
            {
                Configuration cfg = new Configuration();
                cfg.Properties = DbStrategy.NHibernateProperties;

                foreach (KeyValuePair<string, string> import in MappingInfo.QueryLanguageImports)
                    cfg.Imports[import.Key] = import.Value;

                foreach (Assembly assembly in MappingInfo.MappingAssemblies)
                    cfg.AddAssembly(assembly);

                return cfg;
            }


            public override void IntialiseContainerAndUowFactory()
            {
                if (IoC.IsInitialized) IoC.Reset();

                if (RhinoContainer != null)
                {
                    IoC.Initialize(RhinoContainer);
                    NHibernateUnitOfWorkFactory hibernateUnitOfWorkFactory =
                        (NHibernateUnitOfWorkFactory)IoC.Resolve<IUnitOfWorkFactory>();
                    hibernateUnitOfWorkFactory.RegisterSessionFactory(SessionFactory);
                }
            }
        }



        private class ARUnitOfWorkTestContext : UnitOfWorkTestContext
        {
            private IConfigurationSource activeRecordConfigs;
            private ISessionFactory sessionFactory;

            public ARUnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                           string rhinoContainerConfigPath,
                                           MappingInfo assemblies)
                : base(dbStrategy, rhinoContainerConfigPath, assemblies) {}


            private IConfigurationSource ActiveRecordConfigs
            {
                get { return activeRecordConfigs = activeRecordConfigs ?? CreateActiveRecordConfigs(); }
            }

            internal override Configuration Configuration
            {
                get { return ActiveRecordMediator.GetSessionFactoryHolder().GetConfiguration(typeof(ActiveRecordBase)); }
            }


            public override PersistenceFramework Framework
            {
                get { return PersistenceFramework.ActiveRecord; }
            }


            public override ISessionFactory SessionFactory
            {
                get { return sessionFactory = sessionFactory ?? Configuration.BuildSessionFactory(); }
            }


            public override void IntialiseContainerAndUowFactory()
            {
                if (IoC.IsInitialized) IoC.Reset();
                ActiveRecordStarter.ResetInitializationFlag();

                ActiveRecordStarter.Initialize(MappingInfo.MappingAssemblies, ActiveRecordConfigs);
                ActiveRecordMediator.GetSessionFactoryHolder().RegisterSessionFactory(SessionFactory, typeof(ActiveRecordBase));

                if (RhinoContainer != null)
                {
                    IoC.Initialize(RhinoContainer);
                }
            }


            private InPlaceConfigurationSource_AlwaysLazy_AndPluralized CreateActiveRecordConfigs() 
            {
                InPlaceConfigurationSource_AlwaysLazy_AndPluralized cfg = new InPlaceConfigurationSource_AlwaysLazy_AndPluralized();
                cfg.Add(typeof(ActiveRecordBase), DbStrategy.NHibernateProperties);
                return cfg;
            }



            private class InPlaceConfigurationSource_AlwaysLazy_AndPluralized : InPlaceConfigurationSource
            {
                public InPlaceConfigurationSource_AlwaysLazy_AndPluralized()
                {
                    SetIsLazyByDefault(true);
                    SetPluralizeTableNames(true);
                }
            }

        }
    }
}