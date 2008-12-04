using System;

namespace Advance.IoC.ComponentSelectors
{
    public class AuditableNotificatins : INotifications
    {
        private INotifications notifications;

        public AuditableNotificatins(INotifications notifications)
        {
            this.notifications = notifications;
        }

        public void Send()
        {
            Console.WriteLine("called...");
            notifications.Send();
        }
    }
}