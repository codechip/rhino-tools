using System;
using Castle.Windsor;
using HibernatingRhinos.NHibernate.Profiler.Appender;
using HumanResources.Model;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons;
using Rhino.Commons.Facilities;
using Rhino.Security;
using Rhino.Security.Interfaces;

namespace Security.Host
{
    static class Program
    {
        static void Main()
        {
            NHibernateProfiler.Initialize();
            try
            {
                var container = new WindsorContainer();
                IoC.Initialize(container);
                container.Kernel.AddFacility("nh", new NHibernateUnitOfWorkFacility());
                container.Kernel.AddFacility("security", new RhinoSecurityFacility(typeof(Employee)));
                container.AddComponent("entity.extractor", typeof(IEntityInformationExtractor<>),
                                       typeof(EntityInformationExtractor<>));
                container.AddComponent("capture.nh.cfg", typeof(INHibernateInitializationAware),
                                       typeof(CaptureNHibernateConfiguration));

                //CreateDB(container);
                
                using (UnitOfWork.Start())
                using (var tx = UnitOfWork.Current.BeginTransaction())
                {
                    var authorizationRepository = container.Resolve<IAuthorizationRepository>();
                    var authorizationService = container.Resolve<IAuthorizationService>();

                    //authorizationRepository.CreateEntitiesGroup("Salaries");
                    //authorizationRepository.CreateOperation("/Salary/Read");
                    //UnitOfWork.Current.Flush();
                    //authorizationRepository.CreateOperation("/Salary/Write");

                    //var salary = new Salary
                    //{
                    //    HourlyRate = 10,
                    //    Name = "Standard",
                    //};
                    //UnitOfWork.CurrentSession.Save(salary);

                    //UnitOfWork.CurrentSession.Save(new Employee
                    //{
                    //    Name = "ayende",
                    //    Salary = salary
                    //});

                    //var salary = UnitOfWork.CurrentSession
                    //    .CreateCriteria(typeof(Salary))
                    //    .UniqueResult<Salary>();

                    var emp = UnitOfWork.CurrentSession
                        .CreateCriteria(typeof(Employee))
                        .UniqueResult<Employee>();

                    //authorizationRepository.AssociateEntityWith(salary, "Salaries");

                    //var isAllowed = authorizationService.IsAllowed(emp, "/Salary/Read");
                    //Console.WriteLine(isAllowed);

                    var salariesCriteria = UnitOfWork.CurrentSession
                        .CreateCriteria(typeof(Salary))
                        .Add(Restrictions.Like("Name", "Sal", MatchMode.Start));

                    authorizationService
                        .AddPermissionsToQuery(emp, "/Salary/Read", salariesCriteria);

                    foreach (Salary o in salariesCriteria.List())
                    {
                        Console.WriteLine(o.Name);
                    }
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
