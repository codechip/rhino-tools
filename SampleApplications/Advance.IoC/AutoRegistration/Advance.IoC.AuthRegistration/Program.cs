using System;
using System.Collections.Generic;
using Advance.IoC.Components;
using Advance.IoC.Components.Controllers;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.AuthRegistration
{
    class Program
    {
        static void Main()
        {
            var container = InitContainer();

            container.Resolve<Engine>().Run();
        }

        private static IWindsorContainer InitContainer()
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(
                AllTypes
                    .Pick()
                    .FromAssembly(typeof(IController).Assembly)
                    .Unless(type => type.IsAbstract)
                    .WithService.FirstInterface()
                    .Configure(registration =>
                    {
                        if (!typeof(IController).IsAssignableFrom(registration.Implementation))
                            return;

                        registration.LifeStyle.Is(LifestyleType.Transient);
                        string componentName = registration.Implementation.Name;
                        componentName = (componentName.Substring(0, componentName.Length - "Controller".Length));
                        registration.Named(componentName.ToLowerInvariant());
                    })
                );

            return container;
        }
    }
}
