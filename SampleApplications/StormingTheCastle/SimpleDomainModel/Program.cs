using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace SimpleDomainModel
{
    class Program
    {
        static ISessionFactory factory;
        static void Main(string[] args)
        {
            CreateDatabase();

            using (ISession session = factory.OpenSession())
            {
                Customer customer = session.Load<Customer>("ALFKI");
                Console.WriteLine(customer.CompanyName);
                foreach (Order order in customer.Orders)
                {
                    Console.WriteLine("Order #{0}:", order.OrderID);
                    foreach (OrderDetail od in order.OrderDetails)
                    {
                        Console.WriteLine("\tBuy {0} of #{1} ", od.ProductID, od.Quantity);
                    }
                }
            }
        }

        private static void CreateDatabase()
        {
            try
            {
                Configuration cfg = new Configuration();
                cfg.Configure("hibernate.cfg.xml");
                factory = cfg.BuildSessionFactory();

                //new SchemaExport(cfg).Create(true, true);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
