using System;

namespace Advance.IoC.Components.Services
{
    public class DefaultLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}