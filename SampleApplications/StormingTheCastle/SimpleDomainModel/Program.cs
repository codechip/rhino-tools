using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace SimpleDomainModel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Configuration cfg = new Configuration();
                cfg.Configure("hibernate.cfg.xml");
                cfg.BuildSessionFactory();

                new SchemaExport(cfg).Create(true, true);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
