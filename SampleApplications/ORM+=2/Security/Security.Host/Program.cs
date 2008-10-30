using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Castle.Windsor;
using HumanResources.Model;
using log4net.Config;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons;
using Rhino.Commons.Facilities;
using Rhino.Security;
using Rhino.Security.Interfaces;
using Util;

namespace Security.Host
{
    static class Program
    {
        static void Main()
        {
            XmlConfigurator.Configure(new FileInfo("nhprof.log4net.config"));
            try
            {
                var container = new WindsorContainer();
                IoC.Initialize(container);
                container.Kernel.AddFacility("nh", new NHibernateUnitOfWorkFacility());
                container.Kernel.AddFacility("security", new RhinoSecurityFacility(typeof(Employee)));
                container.AddComponent("entity.extractor", typeof (IEntityInformationExtractor<>),
                                       typeof (EntityInformationExtractor<>));
                container.AddComponent("capture.nh.cfg", typeof (INHibernateInitializationAware),
                                       typeof (CaptureNHibernateConfiguration));
                
                //CreateDB(container);

                using(UnitOfWork.Start())
                using(var tx = UnitOfWork.Current.BeginTransaction())
                {
                    var authorizationRepository = container.Resolve<IAuthorizationRepository>();
                    authorizationRepository.CreateOperation("/Salary/Read");
                    UnitOfWork.Current.Flush();
                    authorizationRepository.CreateOperation("/Salary/Write");
                    
                    tx.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        
        }

        private static void CreateDB(IWindsorContainer container)
        {
            using (UnitOfWork.Start())
            {
                var capturedConf = (CaptureNHibernateConfiguration)container.Resolve("capture.nh.cfg");
                new SchemaExport(capturedConf.Configuration).Create(true, true);
            }
        }
    }
}
