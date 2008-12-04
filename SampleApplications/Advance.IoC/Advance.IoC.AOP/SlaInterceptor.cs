using System;
using System.Diagnostics;
using Castle.Core.Interceptor;

namespace Advance.IoC.AOP
{
    public class SlaInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            invocation.Proceed();
            TimeSpan elapsed = stopwatch.Elapsed;
            if(elapsed > TimeSpan.FromSeconds(1))
                throw new TimeoutException("Method took more than a sec: " + elapsed);
            Console.WriteLine("Met SLA, method tool: " + elapsed);
        }
    }
}