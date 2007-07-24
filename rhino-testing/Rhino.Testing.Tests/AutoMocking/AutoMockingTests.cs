using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
    public class AutoMockingTests
    {
        protected AutoMockingContainer _container;
        protected MockRepository _mocks;

        [SetUp]
        public virtual void Setup()
        {
            _mocks = new MockRepository();
            _container = new AutoMockingContainer(_mocks);
            _container.Initialize();
        }
    }
}