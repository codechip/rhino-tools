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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.Windsor;
using MbUnit.Framework;
using NHibernate;
using Rhino.Commons.Facilities;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.ForTesting
{
    using Util;

    public abstract class DatabaseTestFixtureBaseTests : DatabaseTestFixtureBase 
    {
        [SetUp]
        public void TestInitialize()
        {
            //WARNING: ordinarily you would never dispose of your contexts like this between each test,
            //as this would seriously degrade performance.
            //We're only doing it here because we're testing the test framework itself
            IoC.Reset();
            DisposeAndRemoveAllUoWTestContexts();
        }


        [TearDown]
        public void TestCleanup()
        {
            //WARNING: ordinarily you would never dispose of your contexts like this between each test,
            //as this would seriously degrade performance.
            //We're only doing it here because we're testing the test framework itself
            IoC.Reset();
            DisposeAndRemoveAllUoWTestContexts();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSqlCe()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSqlCe);
            VerifyCanCreateUseAndDisposeSession();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSqlCe_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSqlCe);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_SQLite()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.SQLite);
            VerifyCanCreateUseAndDisposeSession();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_SQLite_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.SQLite);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSql2005()
        {
            if (UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled())
            {
                VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSql2005);
                VerifyCanCreateUseAndDisposeSession();
            }
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSql2005_IoC()
        {
            if (UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled())
            {
                VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005);
                VerifyCanCreateUseAndDisposeSession();
                VerifyCanCreateUseAndDisposeUnitOfWork();
            }
        }


        [Test]
        public virtual void EachUnitOfWorkContextConfigurationWillBeCreatedOnlyOnce()
        {
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");

            Assert.AreEqual(1, Contexts.Count);
        }


        [Test]
        public virtual void NewUnitOfWorkContextCreatedForDifferentDatabaseNames()
        {
            if (UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled())
            {
                VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005, "TestDb1");
                VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005, "TestDb2");

                Assert.AreEqual(2, Contexts.Count);
            }
        }


        [Test]
        public virtual void NewUnitOfWorkContextCreatedForDifferentWindorConfigFiles()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.SQLite);
            VerifyCanCreateUnitOfWorkContextFor(AnotherWindsorFilePath, DatabaseEngine.SQLite);

            Assert.AreEqual(2, Contexts.Count);
        }


        [Test]
        public virtual void SwitchingBetweenExistingContextsHasAcceptablePerformace()
        {
            //Creates SQLite context for the first time. Use context to touch all moving parts
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");
            VerifyCanCreateUseAndDisposeUnitOfWork();

            //Create another context and ensure all its component parts are used
            //We're doing this so that the SQLite context created above is no longer current
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.MsSqlCe, "");
            VerifyCanCreateUseAndDisposeUnitOfWork();

            //Reinstate and use existing SQLite context.
            double timing = With.PerformanceCounter(delegate
            {
                InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");
                VerifyCanCreateUseAndDisposeUnitOfWork();
            });

            Assert.Less(timing, 0.2, "reinstating then using existing context sufficiently performant");
        }


        [Test]
        public virtual void CanCreateNestedUnitOfWork()
        {
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");

            VerifyCanCreateUseAndDisposeNestedUnitOfWork();
        }


        [Test]
        public virtual void CallingCreateUnitOfWorkMoreThanOnceIsNotAllowed()
        {
            InitializeNHibernateAndIoC(WindsorFilePath, DatabaseEngine.SQLite, "");

            CurrentContext.CreateUnitOfWork();
            try
            {
                CurrentContext.CreateUnitOfWork();
                Assert.Fail("Exception was expected");
            }
            catch (InvalidOperationException e)
            {
                string message =
                    "Cannot create a nested UnitOfWork with this method. Use CreateNestedUnitOfWork() instead";
                Assert.AreEqual(message, e.Message);
            }
            finally
            {
                CurrentContext.DisposeUnitOfWork();
            }
        }

		[Test]
		public void CanInitializeWithFluentInterfaceAndContainerInstance()
		{
			MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();
			IWindsorContainer container = new WindsorContainer();
			container.AddFacility("nh",
			                      new NHibernateUnitOfWorkFacility(
			                      	new NHibernateUnitOfWorkFacilityConfig(Assembly.GetAssembly(typeof (AREntity)))));

			Initialize(PersistenceFramework.NHibernate, mappingInfo).AndIoC(container);

			Assert.AreSame(container,CurrentContext.RhinoContainer);
		}
		
    	protected void InitializeNHibernateAndIoC(string rhinoContainerPath,
                                                 DatabaseEngine databaseEngine,
                                                 string databaseName)
        {
            InitializeNHibernateAndIoC(FrameworkToTest,
                                      rhinoContainerPath,
                                      databaseEngine,
                                      databaseName,
                                      MappingInfo.FromAssemblyContaining<AREntity>());
        }


        protected void VerifyCanCreateUnitOfWorkContextFor(string rhinoContainerPath,
                                                           DatabaseEngine databaseEngine)
        {
            VerifyCanCreateUnitOfWorkContextFor(FrameworkToTest,
                                                rhinoContainerPath,
                                                databaseEngine,
                                                "");
        }


        protected void VerifyCanCreateUnitOfWorkContextFor(string rhinoContainerPath,
                                                           DatabaseEngine databaseEngine,
                                                           string databaseName)
        {
            VerifyCanCreateUnitOfWorkContextFor(FrameworkToTest,
                                                rhinoContainerPath,
                                                databaseEngine,
                                                databaseName);
        }


        protected void VerifyCanCreateUnitOfWorkContextFor(PersistenceFramework framework,
                                                           string rhinoContainerPath,
                                                           DatabaseEngine databaseEngine,
                                                           string databaseName)
        {
            int nextContextPosition = Contexts.Count;

            //creates the UnitOfWorkContext
            MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();
            InitializeNHibernateAndIoC(framework,
                                      rhinoContainerPath,
                                      databaseEngine,
                                      databaseName,
                                      mappingInfo);

            UnitOfWorkTestContext context = Contexts[nextContextPosition];

            Assert.AreEqual(framework, context.Framework);
            if (rhinoContainerPath != null)
            {
                Assert.AreEqual(rhinoContainerPath, context.RhinoContainerConfigPath);
            }
            else
            {
                Assert.IsEmpty(context.RhinoContainerConfigPath);
            }
            Assert.AreEqual(databaseEngine, context.DatabaseEngine);
            if (string.IsNullOrEmpty(databaseName))
            {
                Assert.AreEqual(
                    NHibernateInitializer.DeriveDatabaseNameFrom(databaseEngine, mappingInfo.MappingAssemblies[0]),
                    context.DatabaseName);
            }
            else
            {
                Assert.AreEqual(databaseName, context.DatabaseName);
            }

            Assert.AreEqual(CurrentContext,
                            context,
                            "Context just built has been assigned to CurrentContext");
        }



        protected void VerifyCanCreateUseAndDisposeSession()
        {
            ISession session = null;
            try
            {
                session = CurrentContext.CreateSession();
                Assert.IsNotNull(session);
                session.Save(new AREntity());
                session.Flush();
            }
            finally
            {
                CurrentContext.DisposeSession(session);
            }
        }


        protected void VerifyCanCreateUseAndDisposeUnitOfWork() {
            try
            {
                CurrentContext.CreateUnitOfWork();
                Console.Write(UnitOfWork.CurrentSession.Connection.ConnectionString);
                UnitOfWork.CurrentSession.Save(new AREntity());
                UnitOfWork.CurrentSession.Flush();
            }
            finally
            {
                CurrentContext.DisposeUnitOfWork();
            }
        }


        protected void VerifyCanCreateUseAndDisposeNestedUnitOfWork() 
        {
            Assert.AreEqual(-1, CurrentContext.UnitOfWorkNestingLevel, "level before starting UoW = -1");

            CurrentContext.CreateUnitOfWork();
            Assert.AreEqual(0, CurrentContext.UnitOfWorkNestingLevel, "level after starting UoW = 0");

            CurrentContext.CreateNestedUnitOfWork();
            Assert.AreEqual(1, CurrentContext.UnitOfWorkNestingLevel, "level after starting nested UoW = 1");

            UnitOfWork.CurrentSession.Save(new AREntity());
            UnitOfWork.CurrentSession.Flush();
            CurrentContext.DisposeUnitOfWork();

            //this is happening in the original UoW
            UnitOfWork.CurrentSession.Save(new AREntity());
            UnitOfWork.CurrentSession.Flush();
            CurrentContext.DisposeUnitOfWork();
        }


        protected abstract string AnotherWindsorFilePath { get; }

        protected abstract PersistenceFramework FrameworkToTest { get; }

        protected abstract string WindsorFilePath { get; }

        protected static string ActiveRecordWindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"ForTesting\Windsor-AR.config"));
            }
        }

        protected static string NHibernateWindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"ForTesting\Windsor-NH.config"));
            }
        }
//		[Test]
//		public virtual void CanInitializeWithFluentNHibernate_Simple()
//		{
//			MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();
//			NHibernateInitializer initializer=Initialize(PersistenceFramework.NHibernate, mappingInfo).Using(DatabaseEngine.SQLite,":memoir");
//			Assert.AreEqual(PersistenceFramework.NHibernate,initializer.PersistenceFramework);
//			Assert.AreSame(mappingInfo,initializer.MappingInfo);
//			Assert.AreEqual(DatabaseEngine.SQLite,initializer.DatabaseEngine);
//			Assert.AreEqual(":memoir",initializer.DatabaseName);
//
//		}
//		[Test]
//		public virtual void CanInitializeWithFluentNHibernate_Complete()
//		{
//			MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();
//			Initialize(PersistenceFramework.NHibernate, mappingInfo);
//			
//		}
//		[Test]
//		public virtual void CanInitializeNHibernateWithIoC()
//		{
//			MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();
//			string rhinoContainerConfig = "windsor.boo";
//			NHibernateInitializer initializer = Initialize(PersistenceFramework.NHibernate, mappingInfo)
//				.AndIoC().With(rhinoContainerConfig);
//			
//		}
    }

    [ActiveRecord]
    public class AREntity
    {
        private Guid id = Guid.NewGuid();
        private int version = -1;
        private string name;
        private int age;

        [PrimaryKey(PrimaryKeyType.Assigned)]
        public virtual Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        [Version(UnsavedValue = "-1")]
        public virtual int Version
        {
            get { return version; }
            set { version = value; }
        }

        [Property]
        public virtual int Age
        {
            get { return age; }
            set { age = value; }
        }

        [Property]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
		
	
}
