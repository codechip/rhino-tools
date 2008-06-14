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
        [Test]
        public void Should_register_NHibernateUnitOfWorkFactory_to_the_container()
        {
            IWindsorContainer sut = CreateSUT();
            sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), sut.Resolve<IUnitOfWorkFactory>());
        }

        [Test]
        public void Should_register_NHRepository_to_the_container()
        {
            IWindsorContainer sut = CreateSUT();
            sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsNotNull(Repository<Parent>.Create());
        }

        [Test]
        public void Should_register_entities_to_the_repository()
        {
            IWindsorContainer sut = CreateSUT();
            sut.AddFacility(facilityKey, new NHibernateUnitOfWorkFacility(GetType().Assembly.FullName));
            Assert.IsNotNull(Repository<IParent>.Create());
        }

        private IWindsorContainer CreateSUT()
        {
            return new WindsorContainer();
        }
    }
}
