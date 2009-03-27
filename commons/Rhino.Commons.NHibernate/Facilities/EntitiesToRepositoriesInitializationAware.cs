using System;
using NHibernate;
using NHibernate.Cfg;

namespace Rhino.Commons.Facilities
{
    internal class EntitiesToRepositoriesInitializationAware : INHibernateInitializationAware 
    {
        private readonly IsCandidateForRepositoryDelegate isCandidateForRepository;

        public EntitiesToRepositoriesInitializationAware(IsCandidateForRepositoryDelegate isCandidateForRepository)
        {
            this.isCandidateForRepository = isCandidateForRepository;
        }

        public void BeforeInitialization()
        {
        }

        public void Configured(Configuration cfg)
        {
        }

        public void Initialized(Configuration cfg, ISessionFactory sessionFactory)
        {
            EntitiesToRepositories.Register(
                IoC.Container,
                sessionFactory,
                typeof(NHRepository<>),
                isCandidateForRepository);
        }
    }
}
