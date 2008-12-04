using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.ComponentSelectors
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Register(
                Component
                    .For<INotifications>()
                    .ImplementedBy<SmsNotifications>()
                    .Named("sms"),
                Component
                    .For<INotifications>()
                    .ImplementedBy<EmailNotifications>()
                    .Named("email"),
                Component
                    .For<INotifications>()
                    .ImplementedBy<AuditableNotificatins>()
                    .Named("auditable"),
                Component
                    .For<IUsersService>()
                    .ImplementedBy<DefaultUsersService>()
                );

            Context.CurrentUser = new User
            {
                Name = "ayende",
                Preferences = new UserPreferences
                {
                    Notification = "auditable.email"
                }
            };
            
            container.Kernel.Resolver
                .AddSubResolver(new NotificationResolver(container));

            var usersService = container.Resolve<IUsersService>();
            Console.WriteLine(usersService.Notifications);
        }
    }

    public class NotificationResolver : ISubDependencyResolver
    {
        private readonly IWindsorContainer container;
        private int level = 0;

        public NotificationResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                              DependencyModel dependency)
        {
            try
            {
                string notification = Context.CurrentUser.Preferences.Notification;
                string split = notification.Split('.')[level];
                level +=1;
                return container.Resolve<INotifications>(split);
            }
            finally
            {
                level -= 1;
            }
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                               DependencyModel dependency)
        {

            return Context.CurrentUser != null &&
                dependency.TargetType == typeof (INotifications);
        }
    }
}