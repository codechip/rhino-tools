using System;
using Advance.IoC.Components.Controllers;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Advance.IoC.Components
{
    public class Engine
    {
        private readonly IKernel container;

        public Engine(IKernel container)
        {
            this.container = container;
        }

        public void Run()
        {
            string input = "login";
            while (input != "q")
            {
                if(container.HasComponent(input))
                {
                    container.Resolve<IController>(input).Execute();
                }
                else
                {
                    Console.WriteLine("Could not find controller named: " + input);
                }
                input = Console.ReadLine();
            }
        }
    }
}