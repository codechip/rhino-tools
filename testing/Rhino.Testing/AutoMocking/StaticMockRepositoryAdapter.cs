using System;
using System.Reflection;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
  /// <summary>Adapter around the static <see cref="MockRepository"/> for using AAA mode.</summary>
  public class StaticMockRepositoryAdapter : IMockRepository
  {
    private static readonly BindingFlags StaticBindingFlags = BindingFlags.Static | BindingFlags.Public |
                                                              BindingFlags.IgnoreCase | BindingFlags.InvokeMethod;

    /// <summary>
    /// Move the repository to ordered mode
    /// </summary>
    public IDisposable Ordered()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Move the repository to un-ordered mode
    /// </summary>
    public IDisposable Unordered()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object CreateMock(Type type, params object[] argumentsForConstructor)
    {
      return typeof (MockRepository).GetMethod("GenerateMock", StaticBindingFlags).MakeGenericMethod(type).Invoke(null, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object StrictMock(Type type, params object[] argumentsForConstructor)
    {
      return StrictMockWithRemoting(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object CreateMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return InvokeGenericMethod("GenerateDynamicMockWithRemoting", type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object StrictMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return InvokeGenericMethod("GenerateStrictMockWithRemoting", type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    public T CreateMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateDynamicMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    public T StrictMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateStrictMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    public object CreateMultiMock(Type mainType, params Type[] extraTypes)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    public object StrictMultiMock(Type mainType, params Type[] extraTypes)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    public object CreateMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    public object StrictMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param>
    public object DynamicMultiMock(Type mainType, params Type[] extraTypes)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    public object DynamicMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object DynamicMock(Type type, params object[] argumentsForConstructor)
    {
      return InvokeGenericMethod("GenerateMock", type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object DynamicMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return InvokeGenericMethod("GenerateDynamicMockWithRemoting", type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    public T DynamicMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateDynamicMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    public object PartialMock(Type type, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param>
    public object PartialMultiMock(Type type, params Type[] extraTypes)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    public object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Cause the mock state to change to replay, any further call is compared to the 
    ///             ones that were called in the record state.
    /// </summary>
    /// <param name="obj">the object to move to replay state</param>
    public void Replay(object obj)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Will delete all current expectations!
    /// </summary>
    public void BackToRecord(object obj)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Optionally, can delete all current expectations, but allows more granularity about how
    ///             it would behave with regard to the object state.
    /// </summary>
    public void BackToRecord(object obj, BackToRecordOptions options)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Verify that all the expectations for this object were fulfilled.
    /// </summary>
    /// <param name="obj">the object to verify the expectations for</param>
    public void Verify(object obj)
    {
      RhinoMocksExtensions.VerifyAllExpectations(obj);
    }

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    public void BackToRecordAll()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    public void BackToRecordAll(BackToRecordOptions options)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Replay all the mocks from this repository
    /// </summary>
    public void ReplayAll()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Verify all the mocks from this repository
    /// </summary>
    public void VerifyAll()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T CreateMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T StrictMock<T>(params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T DynamicMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock object from several types.
    /// </summary>
    public T CreateMultiMock<T>(params Type[] extraTypes) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a strict mock object from several types.
    /// </summary>
    public T StrictMultiMock<T>(params Type[] extraTypes) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    public T DynamicMultiMock<T>(params Type[] extraTypes) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    public T PartialMultiMock<T>(params Type[] extraTypes) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T CreateMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a strict mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T StrictMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a mock object with from a class that defaults to calling the class methods
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T PartialMock<T>(params object[] argumentsForConstructor) where T : class
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Create a stub object, one that has properties and events ready for use, and 
    ///             can have methods called on it. It requires an explicit step in order to create 
    ///             an expectation for a stub.
    /// </summary>
    /// <param name="argumentsForConstructor">The arguments for constructor.</param>
    public T Stub<T>(params object[] argumentsForConstructor) where T : class
    {
      return MockRepository.GenerateStub<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Create a stub object, one that has properties and events ready for use, and
    ///             can have methods called on it. It requires an explicit step in order to create
    ///             an expectation for a stub.
    /// </summary>
    /// <param name="type">The type.</param><param name="argumentsForConstructor">The arguments for constructor.</param>
    /// <returns/>
    public object Stub(Type type, params object[] argumentsForConstructor)
    {
      return InvokeGenericMethod("GenerateStub", type, argumentsForConstructor);
    }

    /// <summary>
    /// Returns true if the passed mock is currently in replay mode.
    /// </summary>
    /// <param name="mock">The mock to test.</param>
    /// <returns>
    /// True if the mock is in replay mode, false otherwise.
    /// </returns>
    public bool IsInReplayMode(object mock)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary/>
    /// <returns/>
    public IDisposable Record()
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary/>
    /// <returns/>
    public IDisposable Playback()
    {
      throw new NotAvailableInAaaModeException();
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