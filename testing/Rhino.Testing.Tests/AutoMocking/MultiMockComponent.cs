using System;

namespace Rhino.Testing.Tests.AutoMocking
{
  public interface IWcfService
  {
    int Add(int x, int y);
  }

  public class WcfProxyImplementation : IWcfService, IDisposable
  {
    public int Add(int x, int y)
    {
      return x + y;
    }

    public void Dispose()
    {
    }
  }

  public class MultiMockComponent : IDisposable
  {
    private IWcfService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Object"/> class.
    /// </summary>
    public MultiMockComponent(IWcfService service)
    {
      _service = service;
    }

    public int Add(int x, int y)
    {
      return _service.Add(x, y);
    }

    public void Dispose()
    {
      var disposableService = _service as IDisposable;
      if (disposableService != null)
      {
        disposableService.Dispose();
      }
    }
  }
}