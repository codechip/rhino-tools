using Rhino.Commons.Facilities;
using Rhino.Commons.Test.Repository;
using Castle.Windsor;
using MbUnit.Framework;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class NHibernateUnitOfWorkFacilityTests
    {
        private static readonly string facilityKey = "nh.uow.facility";
        private string thisAssembly;

        [SetUp]
        public void Setup()
        {
            IoC.Initialize(new WindsorContainer());
            thisAssembly = GetType().Assembly.FullName;
        }

        [TearDown]
        public void Teardown()
        {
            IoC.Reset();
        }

        [Test]
        public void Should_register_NHibernateUnitOfWorkFactory_to_the_container()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(new NHibernateUnitOfWorkFacilityConfig(thisAssembly)));
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), IoC.Resolve<IUnitOfWorkFactory>());
        }

        [Test]
        public void Should_register_NHRepository_to_the_container()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(new NHibernateUnitOfWorkFacilityConfig(thisAssembly)));
            Assert.IsNotNull(Repository<Parent>.Create());
        }

        [Test]
        public void Should_configure_session_factory_with_entities()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(
                new NHibernateUnitOfWorkFacilityConfig(thisAssembly)
                .NHibernateConfiguration("facilities\\hibernate2.cfg.xml")));

            using (UnitOfWork.Start())
                Assert.IsNotNull(UnitOfWork.GetCurrentSessionFor(typeof (Parent)).SessionFactory.GetClassMetadata(typeof(Parent)));
        }

        [Test]
        public void Should_register_entities_to_the_repository()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(
                new NHibernateUnitOfWorkFacilityConfig(thisAssembly)
                .RegisterEntities(true)
                .NHibernateConfiguration("facilities\\hibernate2.cfg.xml")));
            Assert.IsNotNull(Repository<IParent>.Create());
        }
    }
}
