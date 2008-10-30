using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HumanResources.Model;
using log4net.Config;
using Lucene.Net.Analysis;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Search;
using NHibernate.Search.Engine;
using NHibernate.Search.Event;
using NHibernate.Search.Store;
using NHibernate.Tool.hbm2ddl;
using Util;
using Environment=NHibernate.Search.Environment;

namespace Searching.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("nhprof.log4net.config"));

            Configuration cfg = new Configuration()
                .Configure("nhibernate.cfg.xml");

            cfg.SetProperty("hibernate.search.default.directory_provider",
                            typeof (FSDirectoryProvider).AssemblyQualifiedName);
            cfg.SetProperty(Environment.AnalyzerClass,
                            typeof (StopAnalyzer).AssemblyQualifiedName);

            cfg.SetListener(ListenerType.PostUpdate, new FullTextIndexEventListener());
            cfg.SetListener(ListenerType.PostInsert, new FullTextIndexEventListener());
            cfg.SetListener(ListenerType.PostDelete, new FullTextIndexEventListener());

            using (new ConsoleColorer("nhibernate"))
                new SchemaExport(cfg).Execute(true, true, false, true);

            ISessionFactory factory = cfg.BuildSessionFactory();
            using (IFullTextSession s = Search
                .CreateFullTextSession(factory.OpenSession()))
            using (ITransaction tx = s.BeginTransaction())
            {
                s.PurgeAll(typeof (Employee));
                s.PurgeAll(typeof (Salary));

                var salary = new Salary
                                 {
                                     Name = "MinPay",
                                     HourlyRate = 22m
                                 };
                var emp = new Employee
                              {
                                  Name = "ayende",
                                  Salary = salary
                              };
                s.Save(salary);
                s.Save(emp);


                tx.Commit();
            }

            Thread.Sleep(1500);
            Console.Clear();
            using (IFullTextSession s = Search.CreateFullTextSession(factory.OpenSession()))
            using (ITransaction tx = s.BeginTransaction())
            {
                var employees = s.CreateFullTextQuery<Employee>("Name", "a*")
                    .List<Employee>();
                foreach (Employee employee in employees)
                {
                    Console.WriteLine("Employee: " + employee.Name);
                    Console.WriteLine("Salary: {0} - {1:C}",
                                      employee.Salary.Name,
                                      employee.Salary.HourlyRate);
                }

                var salaries = s.CreateFullTextQuery<Salary>("HourlyRate:[20 TO 25]")
                    .List<Salary>();
                foreach (var salary in salaries)
                {
                    Console.WriteLine("Salaray: {0} - {1:C}", salary.Name, salary.HourlyRate);
                }

                tx.Commit();
            }
        }
    }
}