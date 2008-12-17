using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Rhino.ServiceBus.Hosting
{
    public abstract class AbstractBootStrapper
    {
        protected IWindsorContainer container;

        public virtual Assembly Assembly
        {
            get { return GetType().Assembly; }
        }

        public virtual void AfterStart()
        {
        }

        public void InitializeContainer(IWindsorContainer windsorContainer)
        {
            container = windsorContainer;

            ConfigureContainer();
        }

        protected virtual void ConfigureContainer()
        {
            container.Register(
                AllTypes.FromAssembly(Assembly)
                    .BasedOn(typeof(IMessageConsumer))
                );
        }

        public virtual void BeforeStart()
        {
            
        }
    }
}