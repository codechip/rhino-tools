using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Testing.AutoMocking;

namespace Rhino.Testing.Tests.AutoMocking
{
  [TestFixture]
  public class AaaAutoMockingTests
  {
    private AutoMockingContainer _container;

    [SetUp]
    public void SetUp()
    {
      _container = new AutoMockingContainer();
      _container.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
      if (null != _container)
        _container.Dispose();
    }

    [Test]
    public void Can_Create_A_Component_And_Verify_An_Expectation()
    {
      var componentBeingConfigured = _container.Create<ComponentBeingConfigured>();
      var mockReallyCoolService = _container.Get<IReallyCoolService>();

      componentBeingConfigured.ReallyCoolService.GetName();

      mockReallyCoolService.AssertWasCalled(mock => mock.GetName());
    }

    [Test]
    public void Can_Stub_On_A_Mock()
    {
      var component = _container.Create<ComponentBeingConfigured>();
      var service = _container.Get<IReallyCoolService>();

      service.Stub(stub => stub.GetName()).Return("Ayende");

      service.VerifyAllExpectations();
    }

    [Test]
    public void Can_Create_A_Stub()
    {
      _container.Mark<IReallyCoolService>().Stubbed();
      var component = _container.Create<ComponentBeingConfigured>();
      var reallyCoolService = _container.Get<IReallyCoolService>();

      reallyCoolService.GetName();

      reallyCoolService.VerifyAllExpectations();
    }
  }
}