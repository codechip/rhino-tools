namespace Advance.IoC.Components.Services
{
    public interface IStatisticsTracking
    {
        void LoginSuccessful();

        void LoginFailed();
    }
}