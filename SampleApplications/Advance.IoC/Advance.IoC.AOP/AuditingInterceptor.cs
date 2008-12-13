using System;
using System.Threading;
using Castle.Core.Interceptor;

namespace Advance.IoC.AOP
{
    public class AuditingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Method: " + invocation.Method + " was called by " + Thread.CurrentPrincipal.Identity.Name);
            try
            {
                invocation.Proceed();
                Console.WriteLine("Method " + invocation.Method + " succeded!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Method "+invocation.Method +" failed!");
                throw;
            }
               
        }
    }
}