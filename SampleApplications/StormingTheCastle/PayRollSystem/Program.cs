using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Commons;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using PayRollSystem.Northwind;
using PayRollSystem.Southsand;
using PayRollSystem.Common;
using NHibernate.Expression;

namespace PayRollSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IoC.Initialize(new RhinoContainer("windsor.boo"));

                //CreateDatabase();

                //no attempt was made for error handling
                Console.Write("Which client? ");
                string clientName = Console.ReadLine();
                using (Context.Enter(clientName))
                using (UnitOfWork.Start())
                {
                    Console.Write("Employee Id: ");
                    int employeeId = Convert.ToInt32(Console.ReadLine());
                    Employee emp = Repository<Employee>.FindOne(
                            Expression.Eq("Id", employeeId)
                        );
                    
                    Console.WriteLine("Found employee: {0}", emp.Name);
                    Console.Write("How many hours? ");
                    int hours = Convert.ToInt32(Console.ReadLine());
                    Console.Write("How many days? ");
                    int days = Convert.ToInt32(Console.ReadLine());
                    TimeSpan duration = TimeSpan.FromDays(days);

                    decimal salary = emp.CalculateSalary(hours, duration);
                    Console.WriteLine("Earned: {0}$", salary);
                }

            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        private static void CreateDatabase()
        {
            using (Context.Enter("Northwind"))
            {
                new SchemaExport(IoC.Resolve<Configuration>()).Create(true, true);

                using (UnitOfWork.Start())
                {
                    NorthwindEmployee emp = new NorthwindEmployee();
                    emp.HourlySalary = 25;
                    emp.Name = "Northwind Empl";
                    emp.OvertimeBonus = 150;
                    emp.Title = "Mr.";
                    Repository<NorthwindEmployee>.Save(emp);
                    UnitOfWork.Current.TransactionalFlush();
                }
            }

            using (Context.Enter("Southsand"))
            {
                new SchemaExport(IoC.Resolve<Configuration>()).Create(true, true);

                using (UnitOfWork.Start())
                {
                    SouthsandEmployee emp = new SouthsandEmployee();
                    emp.GlobalSalary = 2500;
                    emp.Name = "Southsand Empl";
                    emp.OvertimeHourlySalary = 30;
                    emp.Title = "Mr.";
                    Repository<SouthsandEmployee>.Save(emp);
                    UnitOfWork.Current.TransactionalFlush();
                }
            }
        }
    }
}
