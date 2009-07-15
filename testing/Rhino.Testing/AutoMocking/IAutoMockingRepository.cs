using System;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public interface IAutoMockingRepository
    {
        IMockFactory MockFactory { get; }
        MockRepository MockRepository { get; }
        IKernel Kernel { get; }
        bool CanResolve(Type type);
        object Get(Type type);
				IMockingStrategy DefaultMockingStrategy { get; set;}
        IMockingStrategy GetMockingStrategy(Type type);
        void SetMockingStrategy(Type type, IMockingStrategy strategy);
        void MarkMissing(Type type);
        void AddService(Type type, object service);
        /// <summary>
        /// Call this method to select the mock type (dynamic, stub, etc) that the container should use
        /// when resolving instances of the specfied <paramref name="type"/>
        /// </summary>
        /// <returns><see cref="TypeMarker"/> that you can then used to select the mock type</returns>
        TypeMarker Mark(Type type);
    	bool CanResolveFromMockRepository(Type service);
        bool ResolveProperties { get; set; }
    }

    public interface IGenericMockingRepository
    {
        IMockingStrategy GetMockingStrategy<T>();
        void SetMockingStrategy<T>(IMockingStrategy strategy);
        void AddService<T>(T service);
        T Get<T>() where T : class;
        TypeMarker Mark<T>();
        void MarkMissing<T>();
    }
}
