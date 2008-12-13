using System;
using Advance.IoC.Components.Services;

namespace Advance.IoC.Components.Controllers
{
    public class NewOrderController : IController
    {
        private IAuthenticationService service;

        public NewOrderController(IAuthenticationService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            Console.WriteLine(service != null);
            Console.WriteLine("Ready to accept new order...");
        }
    }
}