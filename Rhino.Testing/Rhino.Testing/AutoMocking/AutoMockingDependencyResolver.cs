using Castle.Core;
using Castle.MicroKernel;

namespace Rhino.Testing.AutoMocking
{
    public class AutoMockingDependencyResolver : ISubDependencyResolver
    {
        private IAutoMockingRepository _autoMock;

        public AutoMockingDependencyResolver(IAutoMockingRepository autoMock)
        {
            _autoMock = autoMock;
        }

        public IAutoMockingRepository AutoMock
        {
            get { return _autoMock; }
        }

        #region ISubDependencyResolver Members

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                               DependencyModel dependency)
        {
            return dependency.DependencyType == DependencyType.Service &&
                   AutoMock.CanResolve(dependency.TargetType);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                              DependencyModel dependency)
        {
            object target = AutoMock.Get(dependency.TargetType);
            if (target != null)
            {
                return target;
            }

            IMockingStrategy strategy = AutoMock.GetMockingStrategy(dependency.TargetType);
            target = strategy.Create(context, dependency.TargetType);
            AutoMock.AddService(dependency.TargetType, target);
            return target;
        }

        #endregion
    }
}