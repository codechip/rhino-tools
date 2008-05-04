using System;
using Castle.Windsor;

namespace Rhino.Commons.Binsor
{
    public class ContainerAdapter
    {
        private readonly IWindsorContainer container;

        public ContainerAdapter(IWindsorContainer container)
        {
            this.container = container;
        }

        public object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        public object Resolve(string serviceName)
        {
            return Container.Resolve(serviceName);
        }

        public object Resolve(Type serviceType, string serviceName)
        {
            return Container.Resolve(serviceName, serviceType);
        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}