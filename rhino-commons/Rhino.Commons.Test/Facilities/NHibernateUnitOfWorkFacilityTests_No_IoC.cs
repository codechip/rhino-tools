using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Facilities;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class NHibernateUnitOfWorkFacilityTests_No_IoC
    {
        private IWindsorContainer container;

        [SetUp]
        public void Setup()
        {
            container = new WindsorContainer();
            container.AddFacility("nh.uow.facility",
                                  new NHibernateUnitOfWorkFacility(
                                      NHibernateUnitOfWorkFacilityTests.BuildFacilityConfiguration()));
        }

        [Test]
        public void Should_register_NHibernateUnitOfWorkFactory_to_the_container()
        {
            Assert.IsInstanceOfType(typeof(NHibernateUnitOfWorkFactory), container.Resolve<IUnitOfWorkFactory>());
        }
    }
}
