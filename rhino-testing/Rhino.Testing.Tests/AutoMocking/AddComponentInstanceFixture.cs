using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
    [TestFixture]
    public class AddComponentInstanceFixture
    {
        private AutoMockingContainer container;
        private MockRepository mocks;

        public class InstancedComponent
        {
        }

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            container = new AutoMockingContainer(mocks);
            container.Initialize();
        }


        [Test]
        public void AddComponentInstance_CanBeRetrieved()
        {
            InstancedComponent component = new InstancedComponent();
            container.Kernel.AddComponentInstance("InstancedComponent", typeof(InstancedComponent), component);
            Assert.AreEqual(component, container.Get<InstancedComponent>());
        }
    }
}