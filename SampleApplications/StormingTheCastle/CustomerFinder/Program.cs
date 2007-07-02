using System;
using System.Collections.Generic;
using System.Threading;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Castle.Windsor;

namespace CustomerFinder
{
    class Program
    {
        static void Main()
        {
            try
            {
                ConfigureDatabase();

                IWindsorContainer container = new WindsorContainer();
                container.AddComponent("logger",typeof(ILogger), typeof(ConsoleLoggerImpl));
                container.AddComponent("auditor", typeof (IAuditor), typeof (AuditorImpl));
                container.AddComponent("authorization", typeof (IAuthorization), typeof (AuthorizationImpl));
                container.AddComponent("repository", typeof(IRepository), typeof(RepositoryImpl));
                container.AddComponent("customer_finder", typeof (ICustomerFinder), typeof (CustomerFinderImpl));

                ICustomerFinder customerFinder = container.Resolve<ICustomerFinder>();
                ICollection<Customer> customers = customerFinder.FindCustomersByName("oren");
                foreach (Customer customer in customers)
                {
                    Console.WriteLine("Got customer: "+customer.Name);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static void ConfigureDatabase()
        {
            ActiveRecordStarter.Initialize(
                InPlaceConfigurationSource.BuildForMSSqlServer("localhost", "test"),
                typeof(Customer),
                typeof(CustomerOperation),
                typeof(Operation));
            ActiveRecordStarter.CreateSchema();
            Customer customer = new Customer();
            customer.Name = "oren";
            ActiveRecordMediator.Save(customer);

            Operation op = new Operation();
            op.Name = "View";
            ActiveRecordMediator.Save(op);

            CustomerOperation co = new CustomerOperation();
            co.Customer = customer;
            co.Operation = op;
            ActiveRecordMediator.Save(co);

        }
    }
}
