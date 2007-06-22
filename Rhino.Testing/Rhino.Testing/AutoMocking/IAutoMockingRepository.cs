using System;
using Castle.MicroKernel;
using Rhino.Mocks;

namespace Rhino.Testing.AutoMocking
{
    public interface IAutoMockingRepository
    {
        MockRepository MockRepository { get; }
        IKernel Kernel { get; }
        IMockingStrategy GetMockingStrategy(Type type);
        void AddService(Type type, object service);
        object Get(Type type);
        T Get<T>();
        void MarkStub(Type type);
        void MarkNonDynamic(Type type);
        void MarkNonMocked(Type type);

        /// <summary>
        /// Will not be resolved (used for optional dependencies)
        /// </summary>
        /// <param name="type"></param>
        void MarkMissing(Type type);

        bool CanResolve(Type type);
    }
}