using System;

namespace Rhino.Testing.AutoMocking
{
  /// <summary>Abstraction which handles creating mock objects</summary>
  public interface IMockFactory
  {
    /// <summary>Creates a Stub or Fake used for arranging / setup of tests.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GenerateStub(Type[] types, params object[] constructorArgs);
    /// <summary>Generates a non-strict &quot;dynamic&quot; mock.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GenerateDynamicMock(Type[] types, params object[] constructorArgs);
    /// <summary>Generates a strict mock</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GenerateStrictMock(Type[] types, params object[] constructorArgs);
    /// <summary>Generates a partial mock useful for testing abstract classes</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GeneratePartialMock(Type[] types, params object[] constructorArgs);
    /// <summary>Generates a strict mock with remoting</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GenerateStrictMockWithRemoting(Type[] types, params object[] constructorArgs);
    /// <summary>Generates a dynamic with remoting.</summary>
    /// <param name="types"></param>
    /// <param name="constructorArgs"></param>
    /// <returns></returns>
    object GenerateDynamicMockWithRemoting(Type[] types, params object[] constructorArgs);
  }
}