using MbUnit.Framework;

namespace Rhino.Testing.Tests.AutoMocking
{
    [TestFixture]
    public class AutoMockingContainerTests : AutoMockingTests
    {
        [Test]
        public void Resolving_ConditionalDependencyWithMockMissing_WillNotResolve()
        {
            container.MarkMissing<ICollectionOfServices>();
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
            Assert.IsNull(target.Services);
        }

        [Test]
        public void Resolving_WithComponent_ReturnsMock()
        {
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

            using (mocks.Unordered())
            {
                target.ReallyCoolService.SayHello();
            }

            mocks.ReplayAll();
            target.ReallyCoolService.SayHello();
            mocks.VerifyAll();

            container.Dispose();
        }

        [Test]
        public void Resolving_WithOtherImplementation_ReturnsMock()
        {
            container.AddComponent("DefaultCollectionOfServices", typeof (ICollectionOfServices),
                                    typeof (DefaultCollectionOfServices));
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

            mocks.ReplayAll();
            Assert.IsInstanceOfType(typeof (DefaultCollectionOfServices), target.Services);
            mocks.VerifyAll();

            container.Dispose();
        }

        [Test]
        public void Resolving_WithComponentWithStub_ReturnsMock()
        {
            container.Mark<ICollectionOfServices>().Stubbed();
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

            using (mocks.Unordered())
            {
                target.Services.SomethingToDispose.Dispose();
            }

            mocks.ReplayAll();
            target.RunDispose();
            mocks.VerifyAll();

            container.Dispose();
        }

        [Test]
        public void Resolving_GetTwice_ReturnsSameMock()
        {
            container.Mark<ICollectionOfServices>().Stubbed();
            ComponentBeingConfigured target1 = container.Create<ComponentBeingConfigured>();
            ComponentBeingConfigured target2 = container.Resolve<ComponentBeingConfigured>();

            Assert.AreEqual(target1, target2);
            Assert.AreEqual(target1.ReallyCoolService, target2.ReallyCoolService);
        }

        [Test]
        public void Get_NotGotten_ReturnsMock()
        {
            Assert.IsNotNull(container.Get<IReallyCoolService>());
        }

        [Test]
        public void Get_AlreadyGotten_ReturnsMock()
        {
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
            Assert.AreEqual(target.ReallyCoolService, container.Get<IReallyCoolService>());
        }
    }
}