using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HibernatingRhinos.NHibernate.Profiler.Appender;
using HumanResources.Model;
using HumanResources.Northwind;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Util;

namespace MultiTenancy.Host
{
    internal class Program
    {
        private static readonly IDictionary<string, ISessionFactory> factories =
            new Dictionary<string, ISessionFactory>(StringComparer.InvariantCultureIgnoreCase);

        private static void Main()
        {
            NHibernateProfiler.Initialize();
            
            BuildFactory("nhibernate");
            BuildFactory("northwind");

            CreateData("nhibernate", 25m);
            CreateData("Northwind", 22m);

            CalculateSalary("nhibernate");
            CalculateSalary("Northwind");
        }

        private static void CreateData(string name, decimal hourlyRate)
        {
            using (new ConsoleColorer(GetColorForName(name)))
            using (ISession session = factories[name].OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                Salary salary;
                if (name == "nhibernate") // probably want a smarter way to do this
                {
                    salary = new Salary
                                 {
                                     HourlyRate = hourlyRate,
                                     Name = "MinPay"
                                 };
                }
                else
                {
                    salary = new SalaryWithOvertimeAndBehavior
                                 {
                                     HourlyRate = hourlyRate,
                                     Name = "WithOvertime",
                                     OvertimeHourlySalary = hourlyRate*2
                                 };
                }
                var emp = new Employee {Name = "ayende", Salary = salary};

                session.Save(salary);
                session.Save(emp);

                session.Save(new TimesheetEntry
                                 {
                                     Employee = emp,
                                     Start = DateTime.Today.AddHours(8),
                                     End = DateTime.Today.AddHours(17)
                                 });

                session.Save(new TimesheetEntry
                                 {
                                     Employee = emp,
                                     Start = DateTime.Today.AddDays(1).AddHours(8),
                                     End = DateTime.Today.AddDays(1).AddHours(19)
                                 });

                tx.Commit();
            }
        }


        private static void CalculateSalary(string name)
        {
            using (new ConsoleColorer(name))
            using (ISession session = factories[name].OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                IList<TimesheetEntry> timesheetEntries = session
                    .CreateCriteria(typeof (TimesheetEntry))
                    .List<TimesheetEntry>();
                var entiresByEmployee = from entry in timesheetEntries
                                        group entry by entry.Employee
                                        into g
                                            select new {Employee = g.Key, Entries = g.AsEnumerable()};
                foreach (var employeeEntry in entiresByEmployee)
                {
                    decimal salary = employeeEntry.Employee.Salary.CalculateSalaryFor(employeeEntry.Entries);
                    Console.WriteLine("Employee: " + employeeEntry.Employee.Name);
                    Console.WriteLine("\tGets: {0:c}", salary);
                }

                tx.Commit();
            }
        }

        private static void BuildFactory(string name)
        {
            Configuration cfg = new Configuration()
                .Configure(name + ".cfg.xml");
            using (new ConsoleColorer(GetColorForName(name)))
                new SchemaExport(cfg).Execute(true, true, false, true);
            factories[name] = cfg.BuildSessionFactory();
        }

        private static ConsoleColor GetColorForName(string name)
        {
            Array values = Enum.GetValues(typeof (ConsoleColor));
            return (ConsoleColor) values.GetValue(Math.Abs(name.GetHashCode())%values.Length);
        }
    }
}