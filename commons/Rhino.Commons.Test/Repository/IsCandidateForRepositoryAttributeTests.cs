using Rhino.Commons;
using MbUnit.Framework;

namespace Rhino.Commons.Test.Repository
{
    [TestFixture]
    public class IsCandidateForRepositoryAttributeTests
    {
        [Test]
        public void The_interface_should_be_a_candidate_for_a_repository()
        { 
            Assert.IsTrue(IsCandidateForRepositoryAttribute.IsCandidate(typeof(AnInterfaceThatIsARepository)));
        }

        [Test]
        public void The_interface_should_not_be_a_candidate_for_a_repository()
        { 
            Assert.IsFalse(IsCandidateForRepositoryAttribute.IsCandidate(typeof(AnInterfaceThatIsNotARepository)));
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
