using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.GenericSpecialization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new WindsorContainer();
            
            container.Register(
                Component.For(typeof (IRepository<>))
                    .ImplementedBy(typeof (DefaultRepository<>)), 
                Component.For<IRepository<User>>()
                    .ImplementedBy<UserRepository>()
                );

            var intRepos = container.Resolve<IRepository<int>>();
            Console.WriteLine(intRepos);
            var stringRepos = container.Resolve<IRepository<string>>();
            Console.WriteLine(stringRepos);
            var userRepos = container.Resolve<IRepository<User>>();
            Console.WriteLine(userRepos);
        }
    }
}