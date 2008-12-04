namespace Advance.IoC.Components.Services
{
    public interface IAuthenticationService
    {
        bool IsValidLogin(string user, string pass);
        void SetPassword(string user, string pass);
    }
}