using System;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
  public interface IMockRepository
  {
    /// <summary>
    /// Move the repository to ordered mode
    /// </summary>
    IDisposable Ordered();

    /// <summary>
    /// Move the repository to un-ordered mode
    /// </summary>
    IDisposable Unordered();

    /// <summary>
    /// Creates a mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object CreateMock(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a strict mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object StrictMock(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object CreateMockWithRemoting(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object StrictMockWithRemoting(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    T CreateMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a strict remoting mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    T StrictMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    object CreateMultiMock(Type mainType, params Type[] extraTypes);

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    object StrictMultiMock(Type mainType, params Type[] extraTypes);

    /// <summary>
    /// Creates a mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    object CreateMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a strict mock from several types, with strict semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    object StrictMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param>
    object DynamicMultiMock(Type mainType, params Type[] extraTypes);

    /// <summary>
    /// Creates a mock from several types, with dynamic semantics.
    ///             Only <paramref name="mainType"/> may be a class.
    /// </summary>
    /// <param name="mainType">The main type to mock.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class.</param>
    object DynamicMultiMock(Type mainType, Type[] extraTypes, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object DynamicMock(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    object DynamicMockWithRemoting(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <typeparam name="T"/><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    /// <returns/>
    T DynamicMockWithRemoting<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    object PartialMock(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param>
    object PartialMultiMock(Type type, params Type[] extraTypes);

    /// <summary>
    /// Creates a mock object that defaults to calling the class methods.
    /// </summary>
    /// <param name="type">Type.</param><param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor.</param>
    object PartialMultiMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor);

    /// <summary>
    /// Cause the mock state to change to replay, any further call is compared to the 
    ///             ones that were called in the record state.
    /// </summary>
    /// <param name="obj">the object to move to replay state</param>
    void Replay(object obj);

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Will delete all current expectations!
    /// </summary>
    void BackToRecord(object obj);

    /// <summary>
    /// Move the mocked object back to record state.
    ///             Optionally, can delete all current expectations, but allows more granularity about how
    ///             it would behave with regard to the object state.
    /// </summary>
    void BackToRecord(object obj, BackToRecordOptions options);

    /// <summary>
    /// Verify that all the expectations for this object were fulfilled.
    /// </summary>
    /// <param name="obj">the object to verify the expectations for</param>
    void Verify(object obj);

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    void BackToRecordAll();

    /// <summary>
    /// All the mock objects in this repository will be moved
    ///             to record state.
    /// </summary>
    void BackToRecordAll(BackToRecordOptions options);

    /// <summary>
    /// Replay all the mocks from this repository
    /// </summary>
    void ReplayAll();

    /// <summary>
    /// Verify all the mocks from this repository
    /// </summary>
    void VerifyAll();

    /// <summary>
    /// Creates a mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T CreateMock<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a strict mock for the spesified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T StrictMock<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a dynamic mock for the specified type.
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T DynamicMock<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Creates a mock object from several types.
    /// </summary>
    T CreateMultiMock<T>(params Type[] extraTypes) where T : class;

    /// <summary>
    /// Creates a strict mock object from several types.
    /// </summary>
    T StrictMultiMock<T>(params Type[] extraTypes) where T : class;

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    T DynamicMultiMock<T>(params Type[] extraTypes) where T : class;

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    T PartialMultiMock<T>(params Type[] extraTypes) where T : class;

    /// <summary>
    /// Create a mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T CreateMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a strict mock object from several types with strict semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T StrictMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a mock object from several types with dynamic semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T DynamicMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a mock object from several types with partial semantics.
    /// </summary>
    /// <param name="extraTypes">Extra interface types to mock.</param><param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T PartialMultiMock<T>(Type[] extraTypes, params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a mock object with from a class that defaults to calling the class methods
    /// </summary>
    /// <param name="argumentsForConstructor">Arguments for the class' constructor, if mocking a concrete class</param>
    T PartialMock<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a stub object, one that has properties and events ready for use, and 
    ///             can have methods called on it. It requires an explicit step in order to create 
    ///             an expectation for a stub.
    /// </summary>
    /// <param name="argumentsForConstructor">The arguments for constructor.</param>
    T Stub<T>(params object[] argumentsForConstructor) where T : class;

    /// <summary>
    /// Create a stub object, one that has properties and events ready for use, and
    ///             can have methods called on it. It requires an explicit step in order to create
    ///             an expectation for a stub.
    /// </summary>
    /// <param name="type">The type.</param><param name="argumentsForConstructor">The arguments for constructor.</param>
    /// <returns/>
    object Stub(Type type, params object[] argumentsForConstructor);

    /// <summary>
    /// Returns true if the passed mock is currently in replay mode.
    /// </summary>
    /// <param name="mock">The mock to test.</param>
    /// <returns>
    /// True if the mock is in replay mode, false otherwise.
    /// </returns>
    bool IsInReplayMode(object mock);

    /// <summary/>
    /// <returns/>
    IDisposable Record();

    /// <summary/>
    /// <returns/>
    IDisposable Playback();
  }
}