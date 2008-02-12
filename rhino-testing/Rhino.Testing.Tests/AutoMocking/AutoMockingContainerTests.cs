using System;
using Castle.MicroKernel;
using MbUnit.Framework;
using Rhino.Mocks;

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
			container.AddComponent("DefaultCollectionOfServices", typeof(ICollectionOfServices),
									typeof(DefaultCollectionOfServices));
			ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

			mocks.ReplayAll();
			Assert.IsInstanceOfType(typeof(DefaultCollectionOfServices), target.Services);
			mocks.VerifyAll();

			container.Dispose();
		}

		[Test]
		public void Resolving_WithComponentWithStub_ReturnsMock()
		{
			container.Mark<ICollectionOfServices>().Stubbed();
			ComponentBeingConfigured target = container.Create<ComponentBeingConfigured>();

			target.Services.SomethingToDispose = mocks.CreateMock<IDisposable>();
			target.Services.SomethingToDispose.Dispose();

			mocks.ReplayAll();
			target.RunDispose();
			mocks.VerifyAll();

			container.Dispose();
		}

		[Test]
		public void Resolving_IKernel_WillReturnKernelInstance()
		{
			NeedIKernel needIKernel = container.Create<NeedIKernel>();
			Assert.AreSame(needIKernel.Kernel, container.Kernel);
		}

		[Test]
		public void Can_Explicitly_Resolve_MockedInstance()
		{
			container.Mark<ICollectionOfServices>().Stubbed();
			container.Resolve<ICollectionOfServices>();
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


		[Test]
		public void Create_SafeToCallMultipleTimes()
		{
			container.Create<ComponentBeingConfigured>();
			container.Create<ComponentBeingConfigured>();
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
		public void Resolving_GetComponentWithComplexProperty_IgnoresProperties()
		{
			ComponentWithComplexProperty target = container.Create<ComponentWithComplexProperty>();
			Assert.IsNull(target.ComplexProperty);
		}

	}
}
