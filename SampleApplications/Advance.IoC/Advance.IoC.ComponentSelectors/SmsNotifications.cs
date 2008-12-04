using System;

namespace Advance.IoC.ComponentSelectors
{
    public class SmsNotifications : INotifications
    {
        public void Send()
        {
            Console.WriteLine("sending via sms");
        }
    }
}