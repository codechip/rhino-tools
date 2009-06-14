using System;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
  public class MockRepositoryAdapter : IMockRepository
  {
    private MockRepository _mockRepository;

    public MockRepositoryAdapter(MockRepository mockRepository)
    {
      _mockRepository = mockRepository;
    }

    /// <summary>
    /// Move the repository to ordered mode
    /// </summary>
    public IDisposable Ordered()
    {
      return _mockRepository.Ordered();
    }

    /// <summary>
    /// Move the repository to un-ordered mode
    /// </summary>
    public IDisposable Unordered()
    {
      return _mockRepository.Unordered();
    }

    /// <summary>
    /// Creates a mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    [Obsolete("Use StrictMock instead")]
    public object CreateMock(Type type, params object[] argumentsForConstructor)
    {
      throw new NotAvailableInAaaModeException();
    }

    /// <summary>
    /// Creates a strict mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object StrictMock(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMock(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    [Obsolete("Use StrictMockWithRemoting instead")]
    public object CreateMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMockWithRemoting(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object StrictMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMockWithRemoting(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    [Obsolete("Use StrictMockWithRemoting instead")]
    public T CreateMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    public T StrictMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    [Obsolete("Use StrictMultiMock instead")]
    public object CreateMultiMock(Type mainType, params Type[] extraTypes)
    {
      return _mockRepository.StrictMultiMock(mainType, extraTypes);
    }

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    public object StrictMultiMock(Type mainType, params Type[] extraTypes)
    {
      return _mockRepository.StrictMultiMock(mainType, extraTypes);
    }

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    [Obsolete("Use StrictMultiMock instead")]
    public object CreateMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMultiMock(mainType, extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    public object StrictMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMultiMock(mainType, extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param>
    public object DynamicMultiMock(Type mainType, params Type[] extraTypes)
    {
      return _mockRepository.DynamicMultiMock(mainType, extraTypes);
    }

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    public object DynamicMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return _mockRepository.DynamicMultiMock(mainType, extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object DynamicMock(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.DynamicMock(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public object DynamicMockWithRemoting(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.DynamicMockWithRemoting(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    public T DynamicMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.DynamicMockWithRemoting<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    public object PartialMock(Type type, params object[] argumentsForConstructor)
    {
      return _mockRepository.PartialMock(type, argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param>
    public object PartialMultiMock(Type type, params Type[] extraTypes)
    {
      return _mockRepository.PartialMultiMock(type, extraTypes);
    }

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    public object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
    {
      return _mockRepository.PartialMultiMock(type, extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Cause the mock state to change to replay, any further call is compared to the 
    ///             ones that were called in the record state.
    /// </summary>
    /// <param name="obj">the object to move to replay state</param>
    public void Replay(object obj)
    {
      _mockRepository.Replay(obj);
    }

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Will delete all current expectations!
    /// </summary>
    public void BackToRecord(object obj)
    {
      _mockRepository.BackToRecord(obj);
    }

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Optionally, can delete all current expectations, but allows more granularity about how
    ///             it would behave with regard to the object state.
    /// </summary>
    public void BackToRecord(object obj, BackToRecordOptions options)
    {
      _mockRepository.BackToRecord(obj, options);
    }

    /// <summary>
    /// Verify that all the expectations for this object were fulfilled.
    /// </summary>
    /// <param name="obj">the object to verify the expectations for</param>
    public void Verify(object obj)
    {
      _mockRepository.Verify(obj);
    }

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    public void BackToRecordAll()
    {
      _mockRepository.BackToRecordAll();
    }

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    public void BackToRecordAll(BackToRecordOptions options)
    {
      _mockRepository.BackToRecordAll(options);
    }

    /// <summary>
    /// Replay all the mocks from this repository
    /// </summary>
    public void ReplayAll()
    {
      _mockRepository.ReplayAll();
    }

    /// <summary>
    /// Verify all the mocks from this repository
    /// </summary>
    public void VerifyAll()
    {
      _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Creates a mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    [Obsolete("Use StrictMock instead")]
    public T CreateMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a strict mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T StrictMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T DynamicMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.DynamicMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Creates a mock object from several types.
    /// </summary>
    [Obsolete("Use StrictMultiMock instead")]
    public T CreateMultiMock<T>(params Type[] extraTypes) where T : class
    {
      return _mockRepository.StrictMultiMock<T>(extraTypes);
    }

    /// <summary>
    /// Creates a strict mock object from several types.
    /// </summary>
    public T StrictMultiMock<T>(params Type[] extraTypes) where T : class
    {
      return _mockRepository.StrictMultiMock<T>(extraTypes);
    }

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    public T DynamicMultiMock<T>(params Type[] extraTypes) where T : class
    {
      return _mockRepository.DynamicMultiMock<T>(extraTypes);
    }

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    public T PartialMultiMock<T>(params Type[] extraTypes) where T : class
    {
      return _mockRepository.PartialMultiMock<T>(extraTypes);
    }

    /// <summary>
    /// Create a mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    [Obsolete("Use StrictMultiMock instead")]
    public T CreateMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMultiMock<T>(extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Create a strict mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T StrictMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.StrictMultiMock<T>(extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.DynamicMultiMock<T>(extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.PartialMultiMock<T>(extraTypes, argumentsForConstructor);
    }

    /// <summary>
    /// Create a mock object with from a class that defaults to calling the class methods
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    public T PartialMock<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.PartialMock<T>(argumentsForConstructor);
    }

    /// <summary>
    /// Create a stub object, one that has properties and events ready for use, and 
    ///             can have methods called on it. It requires an explicit step in order to create 
    ///             an expectation for a stub.
    /// </summary>
    /// <param name="argumentsForConstructor">The arguments for constructor.</param>
    public T Stub<T>(params object[] argumentsForConstructor) where T : class
    {
      return _mockRepository.Stub<T>(argumentsForConstructor);
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
      return _mockRepository.Stub(type, argumentsForConstructor);
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
      return _mockRepository.IsInReplayMode(mock);
    }

    /// <summary/>
    /// <returns/>
    public IDisposable Record()
    {
      return _mockRepository.Record();
    }

    /// <summary/>
    /// <returns/>
    public IDisposable Playback()
    {
      return _mockRepository.Playback();
    }
  }
}