using System;
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
            usersService.Notifications.Send();
        }
    }
}