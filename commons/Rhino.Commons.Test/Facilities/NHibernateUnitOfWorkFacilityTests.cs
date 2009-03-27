using System;
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
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(BuildFacilityConfiguration()));
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), IoC.Resolve<IUnitOfWorkFactory>());
        }

        [Test]
        public void Should_configure_session_factory_with_entities()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(BuildFacilityConfiguration()));

            using (UnitOfWork.Start())
                Assert.IsNotNull(UnitOfWork.GetCurrentSessionFor(typeof(Parent)).SessionFactory.GetClassMetadata(typeof(Parent)));
        }

        [Test]
        public void Should_register_NHRepository_to_the_container()
        {
            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(BuildFacilityConfiguration()));
            Assert.IsNotNull(Repository<Parent>.Create());
        }

        [Test]
        public void Should_register_entities_to_the_repository()
        {
            IsCandidateForRepositoryDelegate entityInThisAssembly = delegate(Type t, Type c) { return t.Assembly == GetType().Assembly; };
            NHibernateUnitOfWorkFacilityConfig config = BuildFacilityConfiguration().RegisterEntitiesWhere(entityInThisAssembly);

            IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(config));

            using (UnitOfWork.Start())
                Assert.IsNotNull(Repository<IParent>.Create());
        }

		[Test]
		public void Should_register_services_with_keys()
		{
			string repositoryKey = "default_repository";
			NHibernateUnitOfWorkFacilityConfig config = BuildFacilityConfiguration().WithRepositoryKey(repositoryKey);
			IoC.Container.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(config));
            Assert.IsTrue(IoC.Container.Kernel.HasComponent(repositoryKey));
		}

        public static NHibernateUnitOfWorkFacilityConfig BuildFacilityConfiguration()
        {
            return new NHibernateUnitOfWorkFacilityConfig(typeof(NHibernateUnitOfWorkFacilityTests).Assembly)
                .NHibernateConfiguration("facilities\\hibernate2.cfg.xml");
        }
    }
}
