using System.IO;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace MultiTenancy.Web.Controllers
{
    public class GeneratorController : Controller
    {
        private readonly Configuration configuration;
        private readonly ISessionFactory sessionFactory;

        public GeneratorController(Configuration configuration, ISessionFactory sessionFactory)
        {
            this.configuration = configuration;
            this.sessionFactory = sessionFactory;
        }

        public ActionResult Schema()
        {
            var writer = new StringWriter();
            using (var session = sessionFactory.OpenSession())
                new SchemaExport(configuration).Execute(true, true, false, true, session.Connection, writer);

            return Content(writer.GetStringBuilder().ToString(),"text/plain");
        }
    }
}