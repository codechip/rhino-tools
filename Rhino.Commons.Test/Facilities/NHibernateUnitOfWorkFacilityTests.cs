using Rhino.Commons.Facilities;
using Rhino.Commons.Test.Repository;
using Castle.Windsor;
using MbUnit.Framework;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class NHibernateUnitOfWorkFacilityTests
    {
        private const string facilityKey = "nh.uow.facility";

		[TearDown]
		public void TearDown()
		{
			IoC.Reset();
		}

        [Test]
        public void Should_register_NHibernateUnitOfWorkFactory_to_the_container()
        {
            IWindsorContainer sut = CreateSUT();
            sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName, true, @"Facilities\hibernate.cfg.xml"));
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), sut.Resolve<IUnitOfWorkFactory>());
        }

        [Test]
        public void Should_register_NHRepository_to_the_container()
        {
            IWindsorContainer sut = CreateSUT();
			sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName, true, @"Facilities\hibernate.cfg.xml"));
            Assert.IsNotNull(Repository<Parent>.Create());
        }

        [Test]
        public void Should_register_entities_to_the_repository()
        {
            IWindsorContainer sut = CreateSUT();
			sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName, true, @"Facilities\hibernate.cfg.xml"));
            Assert.IsNotNull(Repository<IParent>.Create());
        }

        private static IWindsorContainer CreateSUT()
        {
        	WindsorContainer sut = new WindsorContainer();
        	IoC.Initialize(sut);
        	return sut;
        }
    }
}
