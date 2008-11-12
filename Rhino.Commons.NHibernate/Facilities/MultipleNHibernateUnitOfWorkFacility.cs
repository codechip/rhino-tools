using System;
using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using NHibernate;
using NHibernate.Cfg;
using Param = Castle.MicroKernel.Registration.Parameter;

namespace Rhino.Commons.Facilities
{
    public class MultipleNHibernateUnitOfWorkFacility : AbstractFacility
    {
        private readonly NHibernateUnitOfWorkFacilityConfig[] configs;
    	
        public MultipleNHibernateUnitOfWorkFacility(params NHibernateUnitOfWorkFacilityConfig[] configs)
        {
            this.configs = configs;
        }

        protected override void Init()
        {
            Type repositoryInterface = Type.GetType("Rhino.Commons.IRepository`1, Rhino.Commons.NHibernate.Repositories");
            Type repositoryImpl = Type.GetType("Rhino.Commons.NHRepository`1, Rhino.Commons.NHibernate.Repositories");
            if(repositoryImpl!=null)
            {
                Kernel.Register(Component.For(repositoryInterface)
                    .ImplementedBy(repositoryImpl));
            }

            MultipleNHibernateUnitOfWorkFactory unitOfWorkFactory = new MultipleNHibernateUnitOfWorkFactory();
            foreach (NHibernateUnitOfWorkFacilityConfig config in configs)
            {
                NHibernateUnitOfWorkFactory nestedUnitOfWorkFactory = new NHibernateUnitOfWorkFactory(config.NHibernateConfigurationFile);
                nestedUnitOfWorkFactory.RegisterSessionFactory(CreateSessionFactory(config));
                unitOfWorkFactory.Add(nestedUnitOfWorkFactory);
            }
            Kernel.AddComponentInstance<IUnitOfWorkFactory>(unitOfWorkFactory);
        }

        private ISessionFactory CreateSessionFactory(NHibernateUnitOfWorkFacilityConfig config)
        {
            var initializationAwares = Kernel.ResolveAll<INHibernateInitializationAware>();
            foreach (var aware in initializationAwares)
            {
                aware.BeforeInitialization();
            }
            Configuration cfg = new Configuration().Configure(config.NHibernateConfigurationFile);
            foreach (Type mappedEntity in config.Entities) 
                cfg.AddClass(mappedEntity);

            foreach (var aware in initializationAwares)
            {
                aware.Configured(cfg);
            }

            ISessionFactory sessionFactory = cfg.BuildSessionFactory();
            foreach (var aware in initializationAwares)
            {
                aware.Initialized(cfg, sessionFactory);
            }
            return sessionFactory;
        }
    }
}
