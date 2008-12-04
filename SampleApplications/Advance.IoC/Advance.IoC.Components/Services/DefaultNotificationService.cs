using System;

namespace Advance.IoC.Components.Services
{
    public class DefaultNotificationService : INotificationService
    {
        public void Notify(string user, string message)
        {
            Console.WriteLine(user+ ": " + message);
        }
    }
}