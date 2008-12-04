using System;

namespace Advance.IoC.ComponentSelectors
{
    public class EmailNotifications : INotifications
    {
        public void Send()
        {
            Console.WriteLine("sending via email");
        }
    }
}