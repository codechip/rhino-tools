using System;
using System.Reflection;
using NHibernate;
using NHibernate.Criterion;
using Rhino.Commons;
using log4net;
using Castle.MicroKernel;

namespace Rhino.Igloo
{
    internal interface IInjectedEntityStrategy
    {
        bool IsSatisfiedBy(InjectEntityAttribute attribute, PropertyInfo property);
        object GetValueFor(Object id, InjectEntityAttribute attribute, PropertyInfo property);
    }

    internal class InjectedEntityUsingEagerLoadStrategy : IInjectedEntityStrategy
    {
        private readonly IKernel kernel;

        public InjectedEntityUsingEagerLoadStrategy(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public bool IsSatisfiedBy(InjectEntityAttribute attribute, PropertyInfo property)
        {
            return !string.IsNullOrEmpty(attribute.EagerLoad);
        }

        public object GetValueFor(object id, InjectEntityAttribute attribute, PropertyInfo property)
        {
            Type repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            object repository = kernel.Resolve(repositoryType);

            return repositoryType
                .GetMethod("FindOne", new Type[] { typeof(DetachedCriteria) })
                .Invoke(repository, new object[] 
                    { 
                        DetachedCriteria
                            .For(property.PropertyType)
                            .SetFetchMode(attribute.EagerLoad, FetchMode.Eager)
                            .Add(Expression.IdEq(id)) 
                    });
        }
    }

    internal class InjectedEntityUsingFetchingStrategyStrategy : IInjectedEntityStrategy
    {
        private readonly IKernel kernel;

        public InjectedEntityUsingFetchingStrategyStrategy(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public bool IsSatisfiedBy(InjectEntityAttribute attribute, PropertyInfo property)
        {
            return kernel.HasComponent(typeof(IFetchingStrategy<>).MakeGenericType(property.PropertyType));
        }

        public object GetValueFor(object id, InjectEntityAttribute attribute, PropertyInfo property)
        {
            Type repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            object repository = kernel.Resolve(repositoryType);

            return repositoryType
                .GetMethod("FindOne", new Type[] { typeof(DetachedCriteria) })
                .Invoke(repository, new object[] 
                    { 
                        DetachedCriteria
                            .For(property.PropertyType)
                            .Add(Expression.IdEq(id)) 
                    });
        }
    }

    internal class InjectedEntityUsingGetMethodStrategy : IInjectedEntityStrategy
    {
        private readonly IKernel kernel;

        public InjectedEntityUsingGetMethodStrategy(IKernel kernel)
        {
            this.kernel = kernel;
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(InjectedEntityUsingGetMethodStrategy));

        public bool IsSatisfiedBy(InjectEntityAttribute attribute, PropertyInfo property)
        {
            return kernel.HasComponent(typeof(IRepository<>).MakeGenericType(property.PropertyType));
        }

        public object GetValueFor(object id, InjectEntityAttribute attribute, PropertyInfo property)
        {
            log.DebugFormat("Type of property for generic repository is {0}", property.PropertyType);

            Type repositoryType = typeof(IRepository<>).MakeGenericType(property.PropertyType);
            log.DebugFormat("repsitory service is {0}", repositoryType);

            object repository = kernel.Resolve(repositoryType);
            log.DebugFormat("repsitory is {0}", repository.GetType());

            return repositoryType
                .GetMethod("Get")
                .Invoke(repository, new object[] { id });
        }
    }
}
