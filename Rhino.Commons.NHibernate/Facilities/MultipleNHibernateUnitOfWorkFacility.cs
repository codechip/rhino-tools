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
    	private readonly IDictionary<string, IEnumerable<Type>> entityRegistrationRules;

        public MultipleNHibernateUnitOfWorkFacility(IDictionary<string, IEnumerable<Type>> entityRegistrationRules)
        {
            this.entityRegistrationRules = entityRegistrationRules;
        }

        protected override void Init()
        {
            Kernel.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(NHRepository<>)));

            MultipleNHibernateUnitOfWorkFactory unitOfWorkFactory = new MultipleNHibernateUnitOfWorkFactory();
            foreach (KeyValuePair<string, IEnumerable<Type>> kvp in entityRegistrationRules)
            {
				string configurationFileName = kvp.Key;
                NHibernateUnitOfWorkFactory nestedUnitOfWorkFactory = new NHibernateUnitOfWorkFactory(string.Empty);
                nestedUnitOfWorkFactory.RegisterSessionFactory(CreateSessionFactory(configurationFileName, kvp.Value));
                unitOfWorkFactory.Add(nestedUnitOfWorkFactory);
            }
            Kernel.AddComponentInstance<IUnitOfWorkFactory>(unitOfWorkFactory);
        }

        private ISessionFactory CreateSessionFactory(string configurationFile, IEnumerable<Type> mappedEntities)
        {
            Configuration cfg = new Configuration().Configure(configurationFile);
            foreach (Type mappedEntity in mappedEntities) 
                cfg.AddClass(mappedEntity);
            ISessionFactory sessionFactory = cfg.BuildSessionFactory();

            EntitiesToRepositories.Register(Kernel, sessionFactory, typeof(NHRepository<>), delegate { return true; });
            return sessionFactory;
        }
    }
}
