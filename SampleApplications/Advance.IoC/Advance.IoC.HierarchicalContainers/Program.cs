using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.HierarchicalContainers
{
    class Program
    {
        static void Main()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<ISender>()
                    .ImplementedBy<EmailSender>(),
                Component.For<IUsersService>()
                    .ImplementedBy<DefaultUsersService>()
                );

            var childContainer = new WindsorContainer();

            childContainer.Register(
                Component.For<ISender>()    
                    .ImplementedBy<SmsSender>()
                );

            container.AddChildContainer(childContainer);

            Console.WriteLine("Getting sender component");
            var sms1 = container.Resolve<ISender>();
            Console.WriteLine(sms1);
            var sms2 = childContainer.Resolve<ISender>();
            Console.WriteLine(sms2);

            Console.WriteLine();
            Console.WriteLine("Getting user service");

            // trying to get a component defined in the parent container
           // var usersService1 = container.Resolve<IUsersService>();
           // Console.WriteLine(usersService1.Sender);
            
            var usersService2 = childContainer.Resolve<IUsersService>();
            Console.WriteLine(usersService2.Sender);

            }
    }
}
