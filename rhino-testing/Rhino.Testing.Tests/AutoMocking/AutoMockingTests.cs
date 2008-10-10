using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
    public class AutoMockingTests
    {
        protected AutoMockingContainer container;
        protected MockRepository mocks;
        protected AutoMockingContainer containerThatResolvesProperties;
        protected MockRepository mocksForContainerThatResolvesProperties;

        [SetUp]
        public virtual void Setup()
        {
            mocks = new MockRepository();
            container = new AutoMockingContainer(mocks);
            container.Initialize();
            mocksForContainerThatResolvesProperties = new MockRepository();
            containerThatResolvesProperties = new AutoMockingContainer(mocksForContainerThatResolvesProperties, true);
            containerThatResolvesProperties.Initialize();
        }


        [TearDown]
        public void TestCleanup()
        {
            if (container != null) container.Dispose();
            if (containerThatResolvesProperties != null) containerThatResolvesProperties.Dispose();
        }
    }
}
