using System;
using MbUnit.Framework;
using Rhino.Commons.ForTesting;
using Rhino.Commons;
using Rhino.Igloo.Tests;
using Castle.Windsor;
using Rhino.Commons.Facilities;
using Rhino.Igloo;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;

namespace RhinoIglooSample.Test
{
    [TestFixture]
    public class InterfaceInjectionTests
    {
        private readonly Configuration cfg = new Configuration().Configure();

        [SetUp]
        public void Setup()
        {
            IoC.Initialize(new WindsorContainer()
                .AddFacility<NHibernateUnitOfWorkFacility>()
                .AddFacility("mvp", new RhinoIglooFacility(GetType().Assembly))
                .AddComponent<IContextProvider, FakeContextProvider>());
            UnitOfWork.Start();
            new SchemaExport(cfg).Create(false, true);
        }

        [TearDown]
        public void Teardown()
        {
            if(UnitOfWork.IsStarted)
                UnitOfWork.Current.Dispose();
            new SchemaExport(cfg).Drop(false, true);
        }

        private void ConfigureContext(string key)
        {
            FakeContext context = new FakeContext();
            context.Inputs[key] = 1.ToString();
            FakeContextProvider.SetTheCurrentContext(context);
        }

        [Test]
        public void can_resolve_repository()
        {
            Assert.IsNotNull(IoC.Resolve(typeof(IRepository<>).MakeGenericType(typeof(Model.IUser))));
        }

        [Test]
        public void Shoud_find_user_using_get()
        {
            ConfigureContext("UserUsingGet");
            Assert.IsTrue(IoC.Resolve<Controller.UserControllerUsingInterface>().FoundUser);
        }

        [Test]
        public void Shoud_find_user_using_fetching_strategy()
        {
            ConfigureContext("UserUsingFetchingStrategy");
            IoC.Container.AddComponent<IFetchingStrategy<Model.IUser>, Model.StrategyForInterface>();
            Assert.IsTrue(IoC.Resolve<Controller.UserControllerUsingInterface>().FoundUser);
        }
    }
}
