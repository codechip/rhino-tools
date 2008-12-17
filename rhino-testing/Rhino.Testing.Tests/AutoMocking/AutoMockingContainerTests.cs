using System;
using Castle.Core;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Rhino.Testing.Tests.AutoMocking
{
	[TestFixture]
	public class AutoMockingContainerTests : AutoMockingTests
	{
	    [Test]
	    public void Create_WillReturnInstanceOfRequestedComponent()
	    {
	        ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
	        Assert.IsNotNull(target, "component created");
            Assert.IsInstanceOfType(typeof(ComponentBeingConfigured), target, "expected type created");
	    }


	    [Test]
        public void Create_WillResolveDependencies()
	    {
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
            Assert.IsNotNull(target.ReallyCoolService, "mocked ReallyCoolService dependency");
            Assert.IsNotNull(target.Services, "mocked Services dependency");

	    }

        // It might be nice to test this on a class in a separate assembly...
        [Test]
        public void Create_WillResolveDependenciesOnInternalClassWithInternalConstructor()
        {
            InternalComponentBeingConfigured target = container.Create<InternalComponentBeingConfigured>();
            Assert.IsNotNull(target.ReallyCoolService, "mocked ReallyCoolService dependency");
            Assert.IsNotNull(target.Services, "mocked Services dependency");
        }

        [Test]
        public void Create_WillResolveDependenciesPreferringInternalConstructor()
        {
            ComponentBeingConfiguredWithInternalCtor target = container.Create<ComponentBeingConfiguredWithInternalCtor>();
            Assert.IsNotNull(target.ReallyCoolService, "mocked ReallyCoolService dependency");
            Assert.IsNotNull(target.Services, "mocked Services dependency");
        }

        [Test]
        public void Create_WillResolveDependenciesPreferringInternalConstructor_WithResolvePropertiesEnabled()
        {
            ComponentBeingConfiguredWithInternalCtor target = containerThatResolvesProperties.Create<ComponentBeingConfiguredWithInternalCtor>();
            Assert.IsNotNull(target.ReallyCoolService, "mocked ReallyCoolService dependency");
            Assert.IsNotNull(target.Services, "mocked Services dependency");
        }

        [Test]
        public void Create_WillResolveDependenciesPreferringPrivateConstructor()
        {
            ComponentBeingConfiguredWithPrivateCtor target = container.Create<ComponentBeingConfiguredWithPrivateCtor>();
            Assert.IsNotNull(target.ReallyCoolService, "mocked ReallyCoolService dependency");
            Assert.IsNotNull(target.Services, "mocked Services dependency");
        }

        [Test]
        public void Create_WillResolveDependenciesAsMocks_ByDefault()
	    {
            ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
	        VerifyIsMock(target.ReallyCoolService);
	    }

	    [Test]
        public void Create_CanResolveDependenciesAsStubs()
	    {
	        container.Mark<ICollectionOfServices>().Stubbed();
	        ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

	        VerifyIsStub(target.Services);
	    }


	    [Test]
	    public void Create_CanResolveDepdendenciesAsExplicitlyRegisteredTypes()
	    {
	        container.AddComponent("DefaultCollectionOfServices", typeof(ICollectionOfServices),
	                               typeof(DefaultCollectionOfServices));
	        ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

	        Assert.IsInstanceOfType(typeof(DefaultCollectionOfServices), target.Services);
	    }


	    [Test]
	    public void Create_WillIgnorePropertiesWhenResolvingDependencies()
	    {
	        ComponentWithComplexProperty target = container.Create<ComponentWithComplexProperty>();
	        Assert.IsNull(target.ComplexProperty);
	    }


        [Test]
        public void Create_WillNotIgnorePropertiesWhenResolvingDependencies()
        {
            ComponentWithComplexProperty target = containerThatResolvesProperties.Create<ComponentWithComplexProperty>();
            Assert.IsNotNull(target.ComplexProperty);
        }

	    [Test]
	    public void CanStopContainerResolvingSpecificDependencies()
	    {
	        container.MarkMissing<ICollectionOfServices>();
	        ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();
	        Assert.IsNull(target.Services);
	    }


	    [Test]
        public void Create_WillResolveDependencyOnKernelToIntanceFromContainer()
        {
            NeedIKernel needIKernel = container.Create<NeedIKernel>();
            Assert.AreSame(needIKernel.Kernel, container.Kernel);
        }


	    [Test]
        public void Create_WillReturnSingletons_ByDefault()
	    {
	        ComponentBeingConfigured target1 = container.Create<ComponentBeingConfigured>();
	        ComponentBeingConfigured target2 = container.Create<ComponentBeingConfigured>();
	        Assert.AreSame(target1, target2);
	    }


	    [Test]
	    public void Create_CanReturnTransientInstances()
	    {
	        Type targetType = typeof (ComponentBeingConfigured);
            container.AddComponentLifeStyle(targetType.FullName, targetType, LifestyleType.Transient);

	        ComponentBeingConfigured target1 = container.Create<ComponentBeingConfigured>();
	        ComponentBeingConfigured target2 = container.Create<ComponentBeingConfigured>();
	        Assert.AreNotSame(target1, target2);
	    }


	    [Test]
	    public void Create_ReturningTransientInstances_WillResolveDependenciesToTheSameMockObjects()
	    {
	        Type targetType = typeof (ComponentBeingConfigured);
	        container.AddComponentLifeStyle(targetType.FullName, targetType, LifestyleType.Transient);

	        ComponentBeingConfigured target1 = container.Create<ComponentBeingConfigured>();
	        ComponentBeingConfigured target2 = container.Create<ComponentBeingConfigured>();
	        Assert.AreSame(target1.ReallyCoolService, target2.ReallyCoolService);
	    }


	    [Test]
	    public void Create_WillIgnoreLifestyleOfExplictlyRegisteredTypes_WhenResolvingDependencies()
	    {
            //make sure that container will create two instances of ComponentBeingConfigured
            Type targetType = typeof(ComponentBeingConfigured);
            container.AddComponentLifeStyle(targetType.FullName, targetType, LifestyleType.Transient);

            //explictly register dependency to be resolved as transient
            container.AddComponentLifeStyle("DefaultCollectionOfServices", typeof(ICollectionOfServices),
                                   typeof(DefaultCollectionOfServices), LifestyleType.Transient);

	        ComponentBeingConfigured target1 = container.Create<ComponentBeingConfigured>();
	        ComponentBeingConfigured target2 = container.Create<ComponentBeingConfigured>();
	        Assert.AreNotSame(target1, target2, "two instances of components created");
	        Assert.AreSame(target1.Services, target2.Services, "dependencies are same even when this was not requested");
	    }


        [Test]
        public void Get_ReturnsMock_ByDefault()
        {
            VerifyIsMock(container.Get<IReallyCoolService>());
        }


        [Test]
        public void Get_CanReturnStub()
        {
            container.Mark<ICollectionOfServices>().Stubbed();
            VerifyIsStub(container.Get<ICollectionOfServices>());
        }


	    [Test]
        public void Get_Twice_ReturnsSameMock()
        {
            Assert.AreSame(container.Get<IReallyCoolService>(), container.Get<IReallyCoolService>());
        }


	    [Test]
	    public void Get_ReturnsStubs_ThatAreReadyForRecording()
	    {
	        container.Mark<IReallyCoolService>().Stubbed();
	        IReallyCoolService service = container.Get<IReallyCoolService>();

	        SetupResult.For(service.GetName()).Return("Ayende");
	        mocks.ReplayAll();

	        Assert.AreEqual("Ayende", service.GetName());
	    }


	    [Test]
	    public void Get_WillReturnMockEvenForTypesMarkedAsMissing()
	    {
            container.MarkMissing<IReallyCoolService>();
            Assert.IsNotNull(container.Get<IReallyCoolService>());
	    }


	    [Test]
	    public void Resolve_ReturnsMock()
	    {
	        VerifyIsMock(container.Resolve<IReallyCoolService>());
	    }


	    [Test]
	    public void Resolve_CanReturnStub()
	    {
            container.Mark<ICollectionOfServices>().Stubbed();
            VerifyIsStub(container.Resolve<ICollectionOfServices>());
	    }


	    [Test]
        public void Resolve_ReturnsWhatHasBeenGotten()
	    {
	        IReallyCoolService gotten = container.Get<IReallyCoolService>();
	        IReallyCoolService resolved = container.Resolve<IReallyCoolService>();
	        Assert.AreSame(gotten, resolved);
	    }

	    #region Test helpers


	    private void VerifyIsMock(IReallyCoolService mock)
	    {
	        Assert.IsNotNull(mock, "mock is not null");

	        mocks.BackToRecordAll(); // making sure there are no expectations other than the one created here

	        Expect.Call(mock.SayHello);
	        mocks.ReplayAll();
	        VerifyWillThrow<ExpectationViolationException>(
	            mocks.VerifyAll
	        );
	    }


	    private void VerifyIsStub(ICollectionOfServices objectToCheck) 
	    {
	        IDisposable whatever = mocks.StrictMock<IDisposable>();
	        objectToCheck.SomethingToDispose = whatever;

	        Assert.AreSame(objectToCheck.SomethingToDispose, whatever, "stub has properties that behave like properties");
	    }


	    private delegate void VoidMethod();

	    private void VerifyWillThrow<ExpectedException>(VoidMethod proc)
	    {
	        try
	        {
	            proc();
	            Assert.Fail("Exception expected to be thrown");
	        }
	        catch (Exception e)
	        {
	            Assert.IsInstanceOfType(typeof(ExpectedException), e);
	        }
	    }


	    #endregion
	}
}
