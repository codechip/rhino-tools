using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.AOP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<CachingInterceptor>(),
                Component.For<SlaInterceptor>(),
                Component.For<AuditingInterceptor>(),
                Component.For<IRemoteService>()
                    .ImplementedBy<RemoteService>()
                    .Interceptors(
                        new InterceptorReference(typeof(AuditingInterceptor)),
                        new InterceptorReference(typeof(SlaInterceptor)),
                        new InterceptorReference(typeof(CachingInterceptor))
                        )
                    .Anywhere
                );

            var srv = container.Resolve<IRemoteService>();
            srv.ShortOp();
            Console.WriteLine("Short done");
            try
            {
                srv.LongOp();
                Console.WriteLine("long done");
            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
            }
            int op = srv.LongOp();
            Console.WriteLine(op);
        }
    }
}