using System;
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
                AllTypes
					.FromAssembly(Assembly)
					.Where(type => 
						typeof(IMessageConsumer).IsAssignableFrom(type) && 
						IsTypeAcceptableForThisBootStrapper(type)
					)
					.Configure((Action<ComponentRegistration>)ConfigureConsumer)
                );
        }

    	protected virtual void ConfigureConsumer(ComponentRegistration registration)
    	{
    		registration.Named(registration.Implementation.Name);
    	}

    	protected virtual bool IsTypeAcceptableForThisBootStrapper(Type t)
        {
            return true;
        }

        public virtual void BeforeStart()
        {
            
        }
    }
}