using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;

namespace Rhino.Commons
{
    public static class IoC
    {
        private static IWindsorContainer container;
        
        public static void Initialize(IWindsorContainer windsorContainer)
        {
            IoC.container = windsorContainer;
        }
        
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return Container.Resolve<T>(name);
        }
        
        public static IWindsorContainer Container
        {
            get
            {
                if (container == null)
                    throw new InvalidOperationException("The container has not been initialized!");
                return container;
            }
        }
    }
}
