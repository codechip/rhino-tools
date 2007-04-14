using Castle.Core;
using Castle.Core.Interceptor;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Add query caching capabilities
    /// </summary>
    public class CachingInterceptor : IInterceptor, IOnBehalfAware
    {
        private string typeName;

        /// <summary>
        /// Intercepts the specified invocation and adds query caching capabilities
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            using(With.QueryCache(typeName))
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// Sets the intercepted component model.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetInterceptedComponentModel(ComponentModel target)
        {
            typeName = target.Service.GetGenericArguments()[0].FullName;
        }
    }
}