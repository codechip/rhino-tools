using System;
using System.IO;
using AccountingPackage;
using CustomerCareModule;
using HelpDeskPackage;
using HibernatingRhinos.NHibernate.Profiler.Appender;
using Interfaces;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace AdaptiveModelHost
{
    class Program
    {
        static void Main()
        {
            NHibernateProfiler.Initialize();
            
            DemoUsingCustomerCareModule<AccountingCustomer>(
                "Accounting");

           DemoUsingCustomerCareModule<HelpDeskCustomer>("HelpDesk");
        }

        public static void DemoUsingCustomerCareModule<TCustomer>(string schema)
            where TCustomer : ICustomer, new()
        {
            Configuration cfg = new Configuration()
                .Configure("hibernate.cfg.xml")
                .AddAssembly(typeof(Lead).Assembly)
                .AddAssembly(typeof(TCustomer).Assembly);

            cfg.MapManyToOne<ICustomer, TCustomer>();
            cfg.SetSchema(schema);

            new SchemaExport(cfg).Execute(false, true, false, true);

            ISessionFactory factory = cfg.BuildSessionFactory();

            object accountCustomerId;
            using (var session = factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var customer = new TCustomer { Name = "ayende"};
                session.Save(customer);
                var customerCareService = new CustomerCareService(session);
                customerCareService.GenerateLeadFor(customer, "phone call");
                customerCareService.GenerateLeadFor(customer, "email ");

                tx.Commit();

                accountCustomerId = session.GetIdentifier(customer);
            }

            using (var session = factory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var customer = session.Get<TCustomer>(accountCustomerId);
                var customerCareService = new CustomerCareService(session);

                Console.WriteLine("Leads for: " + customer.Name);
                foreach (var lead in customerCareService.GetLeadsFor(customer))
                {
                    Console.WriteLine("\t" + lead.Note);
                }

                tx.Commit();
            }
        }
    }
}
