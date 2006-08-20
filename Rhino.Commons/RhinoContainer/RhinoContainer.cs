using System;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;

namespace Rhino.Commons
{
    public class RhinoContainer : WindsorContainer
    {
        private ObjectInstanceResolver objectInstanceResolver;

        public RhinoContainer()
        {
            Initializer();
        }
        
        public RhinoContainer(string fileName) 
            : this(new XmlInterpreter(fileName))
        {
        }

        public RhinoContainer(IConfigurationInterpreter interpreter) : base()
        {
            Initializer();
            interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
            RunInstaller();
        }

        private void Initializer()
        {
            objectInstanceResolver = new ObjectInstanceResolver();
            Kernel.Resolver.AddSubResolver(objectInstanceResolver);
        }
        
        public void RegisterDependencyItem(IHandler handler, string dependencyKey,
            object instance)
        {
            Validation.NotNull(handler, "handler");
            Validation.NotNull(dependencyKey, "dependencyKey");
            Validation.NotNull(instance, "instance");
            ComponentModel model = handler.ComponentModel;
            foreach (ConstructorCandidate constructor in model.Constructors)
            {
                foreach (DependencyModel dependency in constructor.Dependencies)
                {
                    if (TryAddDependencyInstnace(handler, dependency, dependencyKey, instance))
                        return;
                }
            }
            foreach (DependencyModel dependency in model.Dependencies)
            {
                if (TryAddDependencyInstnace(handler, dependency, dependencyKey, instance))
                    return;
            }
            throw new InvalidOperationException(string.Format("Could not find dependency {0} on model {1}", dependencyKey, model.Name));
        }

        private bool TryAddDependencyInstnace(IHandler handler, DependencyModel dependency, string dependencyKey, object instance)
        {
            if(dependency.DependencyKey == dependencyKey)
            {
                objectInstanceResolver.AddDependencyInstance(dependency, instance);
                RecalculateHandlerDependencies(handler,dependency);
                return true;
            }
            return false;
        }

        private void RecalculateHandlerDependencies(IHandler handler,DependencyModel dependency)
        {
            AbstractHandler abstractHandler = handler as AbstractHandler;
            if(abstractHandler==null)
            {
                throw new InvalidOperationException(string.Format("Custom handler {0} does not inherit from abstract handler. Can't remove dependency from handler!", handler.GetType()));
            }
			abstractHandler.RemovedDependency(dependency);
            
        }
    }
}