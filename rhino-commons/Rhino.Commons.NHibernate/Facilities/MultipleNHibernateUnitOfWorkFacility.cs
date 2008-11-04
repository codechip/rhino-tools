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
            Kernel.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NHRepository<>)));

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
            Configuration cfg = new Configuration().Configure(config.NHibernateConfigurationFile);
            foreach (Type mappedEntity in config.Entities) 
                cfg.AddClass(mappedEntity);
            
            ISessionFactory sessionFactory = cfg.BuildSessionFactory();
            EntitiesToRepositories.Register(Kernel, sessionFactory, typeof(NHRepository<>), config.IsCandidateForRepository);
            return sessionFactory;
        }
    }
}
