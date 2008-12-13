using System;
using Advance.IoC.Components.Services;

namespace Advance.IoC.Components.Controllers
{
    public class ForgotMyPassController : IController
    {
        public void Execute()
        {
            Console.WriteLine("Forgot my password called");
        }
    }
}