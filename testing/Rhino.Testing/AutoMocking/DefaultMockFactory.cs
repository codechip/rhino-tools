using System;
using System.Linq;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
  /// <summary>Default implementation of a <see cref="IMockFactory"/> that just delegates
  /// to an instance of a mock repository.</summary>
  public class DefaultMockFactory : IMockFactory
  {
    private readonly MockRepository _mocks;

    /// <summary>Creates a new Mock Factory</summary>
    /// <param name="mockRepository"></param>
    public DefaultMockFactory(MockRepository mockRepository)
    {
      _mocks = mockRepository;
    }

    /// <summary>Creates a Stub or Fake used for arranging / setup of tests.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStub(Type[] types, params object[] constructorArgs)
    {
      return _mocks.Stub(types[0], constructorArgs);
    }

    /// <summary>Generates a non-strict &quot;dynamic&quot; mock.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateDynamicMock(Type[] types, params object[] constructorArgs)
    {
      if (types.Length > 1)
      {
        return _mocks.DynamicMultiMock(types[0], types.Skip(1).ToArray(), constructorArgs);
      }
      return _mocks.DynamicMock(types[0], constructorArgs);
    }

    /// <summary>Generates a strict mock</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStrictMock(Type[] types, params object[] constructorArgs)
    {
      if (types.Length > 1)
      {
        return _mocks.StrictMultiMock(types[0], types.Skip(1).ToArray(), constructorArgs);
      }
      return _mocks.StrictMock(types[0], constructorArgs);
    }

    /// <summary>Generates a partial mock useful for testing abstract classes</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GeneratePartialMock(Type[] types, params object[] constructorArgs)
    {
      if (types.Length > 1)
      {
        return _mocks.PartialMultiMock(types[0], types.Skip(1).ToArray(), constructorArgs);
      }
      return _mocks.PartialMock(types[0], constructorArgs);
    }

    /// <summary>Generates a strict mock with remoting</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStrictMockWithRemoting(Type[] types, params object[] constructorArgs)
    {
      return _mocks.StrictMockWithRemoting(types[0], constructorArgs);
    }

    /// <summary>Generates a dynamic with remoting.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateDynamicMockWithRemoting(Type[] types, params object[] constructorArgs)
    {
      return _mocks.DynamicMockWithRemoting(types[0], constructorArgs);
    }
  }
}