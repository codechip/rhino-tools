using System;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
  /// <summary>AAA implementation of a <see cref="IMockFactory"/> that uses the static GenerateXXX methods on a <see cref="MockRepository"/>.</summary>
// ReSharper disable InconsistentNaming
  public class AAAMockFactory : IMockFactory
// ReSharper restore InconsistentNaming
  {
    private static readonly BindingFlags StaticBindingFlags = BindingFlags.Static | BindingFlags.Public |
                                                              BindingFlags.IgnoreCase | BindingFlags.InvokeMethod;

    /// <summary>Creates a Stub or Fake used for arranging / setup of tests.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStub(Type[] types, params object[] constructorArgs)
    {
      return MockRepository.GenerateStub(types[0], constructorArgs);
    }

    /// <summary>Generates a non-strict &quot;dynamic&quot; mock.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateDynamicMock(Type[] types, params object[] constructorArgs)
    {
      return MockRepository.GenerateMock(types[0], types.Skip(1).ToArray(), constructorArgs);
    }

    /// <summary>Generates a strict mock</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStrictMock(Type[] types, params object[] constructorArgs)
    {
      return MockRepository.GenerateStrictMock(types[0], types.Skip(1).ToArray(), constructorArgs);
    }

    /// <summary>Generates a partial mock useful for testing abstract classes</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GeneratePartialMock(Type[] types, params object[] constructorArgs)
    {
      return MockRepository.GeneratePartialMock(types[0], types.Skip(1).ToArray(), constructorArgs);
    }

    /// <summary>Generates a strict mock with remoting</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateStrictMockWithRemoting(Type[] types, params object[] constructorArgs)
    {
      return InvokeGenericMethod("GenerateStrictMockWithRemoting", types[0], constructorArgs);
    }

    /// <summary>Generates a dynamic with remoting.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    public object GenerateDynamicMockWithRemoting(Type[] types, params object[] constructorArgs)
    {
      return InvokeGenericMethod("GenerateDynamicMockWithRemoting", types[0], constructorArgs);
    }

    private static object InvokeGenericMethod(string methodName, Type type, object[] argumentsForConstructor)
    {
      var methods = typeof(MockRepository).GetMethods(StaticBindingFlags);
      MethodInfo method = null;
      foreach (var methodInfo in methods)
      {
        if (methodInfo.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase) && methodInfo.IsGenericMethodDefinition)
        {
          method = methodInfo;
          break;
        }
      }

      if (null == method)
      {
        throw new InvalidOperationException("Could not find a matching generic method for" + methodName);
      }
      var genericMethod = method.MakeGenericMethod(type);
      return genericMethod.Invoke(null, new object[] { argumentsForConstructor });
    }
  }
}