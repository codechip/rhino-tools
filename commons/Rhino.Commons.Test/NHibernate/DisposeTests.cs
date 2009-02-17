using System;
using System.IO;
using System.Threading;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Facilities;
using Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts;

namespace Rhino.Commons.Test.NHibernate
{
    [TestFixture]
    class DisposeTests
    {

        private Thread t;
        		
        [SetUp()]
        public void Setup()
        {
            t = new Thread(DoWork);             
        }
        [TearDown]
        public void TearDown()
        {
            t.Abort();
            IoC.Reset();
        }

        void ThreadSetup()
        {
            InitializeIOC();
        }

        private void InitializeIOC()
            {
                string directory = Path.Combine(System.Environment.CurrentDirectory,
                                                @"NHibernate");

                IoC.Initialize(
                    new WindsorContainer()
                        .AddFacility("Unit.Of.Work", new NHibernateUnitOfWorkFacility(CreateRules())));
            }

        private NHibernateUnitOfWorkFacilityConfig CreateRules()
            {
                string directory = Path.Combine(System.Environment.CurrentDirectory,
                                                @"NHibernate");

                return
                    new NHibernateUnitOfWorkFacilityConfig()
                        .AddEntity(typeof(DomainObjectFromDatabase1))
                        .NHibernateConfiguration(Path.Combine(directory, "Database1.cfg.xml"))
                        .RegisterEntitiesWhere(delegate(Type t) { return true; });
            }

        private void DoWork()
            {
                ThreadSetup();

                using (UnitOfWork.Start())
                {
                    Assert.IsTrue(MockCacheProvider.IsInitialized);
                    Assert.IsTrue(MockCacheProvider.IsRunning);
                    Assert.IsFalse(MockCacheProvider.IsStopped);
                }
            }

        private void PerformTest(bool dispose)
        {
            Assert.IsFalse(MockCacheProvider.IsInitialized);
            Assert.IsFalse(MockCacheProvider.IsRunning);
            Assert.IsTrue(MockCacheProvider.IsStopped);

            t.Start();

            while (t.IsAlive)
            {
                Thread.Sleep(100);
            }

            if (dispose)
                UnitOfWork.DisposeUnitOfWorkFactory();
            Thread.Sleep(250); //wait for it

        }
        [Test]
        public void TestFailingImplicitDispose()
        {
            PerformTest(false);
            Assert.IsTrue(MockCacheProvider.IsRunning, "Maintenance Thread is still Running!");
            Assert.IsFalse(MockCacheProvider.IsStopped, "Cache is not stopped!"); 
        }

        [Test]
        public void TestMockCacheProvider()
        {
            PerformTest(true);
            Assert.IsFalse(MockCacheProvider.IsRunning, "Maintenance Thread is still Running!");
            Assert.IsTrue(MockCacheProvider.IsStopped, "Cache is not stopped!"); 
        }
    }
}
            
    

