using Castle.Core;
using Castle.MicroKernel;
using NHibernate;

namespace Rhino.ServiceBus.NHibernate.Resolvers
{
    public class SessionResolver : ISubDependencyResolver
    {
        private readonly ISessionFactory sessionFactory;

        public SessionResolver(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            if (dependency.TargetType == typeof(ISession))
                return sessionFactory.OpenSession();
            return sessionFactory.OpenStatelessSession();
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof (ISession) |
                   dependency.TargetType == typeof (IStatelessSession);
        }
    }
}