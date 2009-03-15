using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Dialect;
using Environment=NHibernate.Cfg.Environment;

namespace MultiTenancy.Web.Context
{
    public abstract class AbstractBootStrapper : IBootStrapper
    {
        private IWindsorContainer container;
        public RootContext RootContext { get; private set; }
        public abstract string TenantId { get; }

        protected virtual Assembly Assembly
        {
            get { return GetType().Assembly; }
        }

        #region IBootStrapper Members

        public void Init(RootContext context)
        {
            RootContext = context;
        }

        public TenantContext CreateContext()
        {
            container = CreateContainer();
            ConfigureContainer();
            ConfigureNHibernate();
            return new TenantContext(container, Assembly.GetName().Name);
        }

        #endregion

        protected virtual void ConfigureNHibernate()
        {
            var configuration = new Configuration()
                .SetProperties(new Dictionary<string, string>
                {
                    {Environment.Dialect, typeof (MsSql2005Dialect).AssemblyQualifiedName},
                    {Environment.ProxyFactoryFactoryClass, typeof (ProxyFactoryFactory).AssemblyQualifiedName},
                    {Environment.ConnectionString, RootContext.GetConnectionStringFor(TenantId)},
                });
            var customMapping = GetMappingFrom(Assembly);
            var added = new HashSet<string>();
            foreach (var mapping in customMapping)
            {
                configuration.AddResource(mapping, Assembly);
                added.Add(GetEntityName(mapping));
            } 
            var coreMapping = GetMappingFrom(typeof(AbstractBootStrapper).Assembly);
            foreach (var mapping in coreMapping)
            {
                if (added.Add(GetEntityName(mapping)) == false)
                    continue;//already there
                configuration.AddResource(mapping, typeof (AbstractBootStrapper).Assembly);
            }

            container.Kernel.AddComponentInstance<Configuration>(configuration);

            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            container.Kernel.AddComponentInstance<ISessionFactory>(sessionFactory);
        }

        private string GetEntityName(string mapping)
        {
            var replace = mapping.Replace(".hbm.xml", "");
            var namespaceEnd = replace.IndexOf("Model.");
            return replace.Substring(namespaceEnd);
        }

        private static IEnumerable<string> GetMappingFrom(Assembly assembly)
        {
            return from mapping in assembly.GetManifestResourceNames()
                   where mapping.EndsWith(".hbm.xml")
                   select mapping;
        }

        protected virtual void ConfigureContainer()
        {
            container.Register(
                AllTypes.Of<IController>()
                    .FromAssembly(Assembly)
                    .Configure(registration =>
                    {
                        var name = registration.Implementation.Name;
                        registration.Named(name.Replace("Controller", "").ToLowerInvariant());
                    }),

                 AllTypes.Of<IController>()
                    .FromAssembly(typeof(AbstractBootStrapper).Assembly)
                    .Configure(registration =>
                    {
                        var name = registration.Implementation.Name;
                        registration.Named(name.Replace("Controller", "").ToLowerInvariant());
                    }),

                AllTypes.FromAssembly(Assembly)
                    .Where(x=>x.Namespace.EndsWith("Services"))
                    .WithService.FirstInterface(),

                AllTypes.FromAssembly(typeof(AbstractBootStrapper).Assembly)
                    .Where(x=>x.Namespace.EndsWith("Services"))
                    .WithService.FirstInterface()
                );
        }

        protected virtual WindsorContainer CreateContainer()
        {
            return new WindsorContainer();
        }
    }
}