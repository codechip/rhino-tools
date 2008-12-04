using System;
using Castle.Core.Interceptor;

namespace Advance.IoC.AOP
{
    public class CachingInterceptor : IInterceptor
    {
        private object cacheValue;
        
        public void Intercept(IInvocation invocation)
        {
            if (cacheValue != null)
            {
                Console.WriteLine("Using cached value");
                invocation.ReturnValue = cacheValue;
                return;
            }
            invocation.Proceed();
            cacheValue = invocation.ReturnValue;
        }
    }
}