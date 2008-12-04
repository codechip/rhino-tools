namespace Advance.IoC.ComponentSelectors
{
    public class DefaultUsersService : IUsersService
    {
        private readonly INotifications notifications;

        public INotifications Notifications
        {
            get { return notifications; }
        }

        public DefaultUsersService(INotifications notifications)
        {
            this.notifications = notifications;
        }

        public void ForgotMyPasswod()
        {
            
        }
    }
}