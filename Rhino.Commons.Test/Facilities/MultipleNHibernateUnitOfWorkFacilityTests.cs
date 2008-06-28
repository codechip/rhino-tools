using MbUnit.Framework;
using Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class MultipleNHibernateUnitOfWorkFacilityTests : MultipleNHibernateUnitOfWorkTestBase
    {
        [Test]
        public void Should_be_able_to_get_domain_objects_from_multiple_databases()
        {
            Assert.IsNotNull(Repository<DomainObjectFromDatabase1>.Get(1));
            Assert.IsNotNull(Repository<DomainObjectFromDatabase2>.Get(1));
        }
    }
}
