namespace Advance.IoC.ComponentSelectors
{
    public interface IUsersService
    {
        INotifications Notifications { get; }
        void ForgotMyPasswod();
    }
}