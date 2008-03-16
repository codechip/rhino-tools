using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
    public class AutoMockingTests
    {
        protected AutoMockingContainer container;
        protected MockRepository mocks;

        [SetUp]
        public virtual void Setup()
        {
            mocks = new MockRepository();
            container = new AutoMockingContainer(mocks);
            container.Initialize();
        }


        [TearDown]
        public void TestCleanup()
        {
            if (container != null) container.Dispose();
        }
    }
}