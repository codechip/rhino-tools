using NHibernate;
using NHibernate.Cfg;
using Rhino.Commons;

namespace Security.Host
{
    public class CaptureNHibernateConfiguration : INHibernateInitializationAware
    {
        public void BeforeInitialization()
        {
            
        }

        public void Configured(Configuration cfg)
        {
        }

        public Configuration Configuration { get; set; }

        public void Initialized(Configuration cfg, ISessionFactory sessionFactory)
        {
            Configuration = cfg;
        }
    }
}