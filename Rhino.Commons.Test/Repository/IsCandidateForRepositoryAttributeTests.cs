using Rhino.Commons;
using MbUnit.Framework;
using Rhino.Commons.Facilities;

namespace Rhino.Commons.Test.Repository
{
    [TestFixture]
    public class IsCandidateForRepositoryAttributeTests
    {
        [Test]
        public void The_interface_should_be_a_candidate_for_a_repository()
        {
            var config = new NHibernateUnitOfWorkFacilityConfig();
            Assert.IsTrue(config.IsCandidateForRepository(typeof(AnInterfaceThatIsARepository)));
        }

        [Test]
        public void The_interface_should_not_be_a_candidate_for_a_repository()
        {
            var config = new NHibernateUnitOfWorkFacilityConfig();
            Assert.IsFalse(config.IsCandidateForRepository(typeof(AnInterfaceThatIsNotARepository)));
        }
    }

    [IsCandidateForRepository]
    public interface AnInterfaceThatIsARepository
    { 
    }

    public interface AnInterfaceThatIsNotARepository
    {
    }
}
