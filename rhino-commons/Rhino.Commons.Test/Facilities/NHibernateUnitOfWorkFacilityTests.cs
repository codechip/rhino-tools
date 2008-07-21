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

        [SetUp]
        public void Setup()
        {
            IoC.Initialize(new WindsorContainer());
        }

        [TearDown]
        public void Teardown()
        {
            IoC.Reset();
        }

        [Test]
        public void Should_register_NHibernateUnitOfWorkFactory_to_the_container()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), IoC.Resolve<IUnitOfWorkFactory>());
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsInstanceOfType(typeof (NHibernateUnitOfWorkFactory), IoC.Resolve<IUnitOfWorkFactory>());
        }

        [Test]
        public void Should_register_NHRepository_to_the_container()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsNotNull(Repository<Parent>.Create());
        }

        [Test, Ignore]
        public void Should_configure_session_factory_with_entities()
        {
			IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName, "facilities\\hibernate2.cfg.xml"));

            using (UnitOfWork.Start())
                Assert.IsNotNull(UnitOfWork.GetCurrentSessionFor(typeof (Parent)).SessionFactory.GetClassMetadata(typeof(Parent)));
        }

        [Test]
        public void Should_register_entities_to_the_repository()
        {
			IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName, true, "facilities\\hibernate2.cfg.xml"));
            Assert.IsNotNull(Repository<IParent>.Create());
        }
    }
}
