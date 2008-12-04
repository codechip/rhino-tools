using System;

namespace Advance.IoC.Components.Services
{
    public class DefaultStatisticsTracking : IStatisticsTracking
    {
        public void LoginSuccessful()
        {
            Console.WriteLine("Login successful");
        }

        public void LoginFailed()
        {
            Console.WriteLine("Login failed!");
        }
    }
}