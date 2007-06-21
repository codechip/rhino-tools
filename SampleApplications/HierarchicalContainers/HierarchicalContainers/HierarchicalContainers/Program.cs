using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.Windsor;
using Rhino.Commons;

namespace HierarchicalContainers
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RhinoContainer container = new RhinoContainer("Windsor.boo");
                IoC.Initialize(container);
                ContainerSelector containerSelector = IoC.Resolve<ContainerSelector>();
                containerSelector.PrintChildContainers();
                using(UnitOfWork.Start())
                {
                    Console.WriteLine(
                        NHibernateUnitOfWorkFactory.CurrentNHibernateSession
                            .Connection.ConnectionString
                        );
                }
                using(containerSelector.Enter("Northwind"))
                {
                    using (UnitOfWork.Start())
                    {
                        Console.WriteLine(
                            NHibernateUnitOfWorkFactory.CurrentNHibernateSession
                                .Connection.ConnectionString
                            );
                    }
                }
                using (containerSelector.Enter("Southsand"))
                {
                    using (UnitOfWork.Start())
                    {
                        Console.WriteLine(
                            NHibernateUnitOfWorkFactory.CurrentNHibernateSession
                                .Connection.ConnectionString
                            );
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

     
    }

    public class ContainerSelector
    {
        private readonly IDictionary<string, IWindsorContainer> containers = new Dictionary<string, IWindsorContainer>();

        public void Register(string name, IWindsorContainer container)
        {
            containers.Add(name, container);
        }

        public void PrintChildContainers()
        {
            foreach (KeyValuePair<string, IWindsorContainer> pair in containers)
            {
                Console.WriteLine("Container {0}", pair.Key);
            }
        }

        public IDisposable Enter(string name)
        {
            if(containers.ContainsKey(name)==false)
            {
                throw new InvalidOperationException("Container " + name + " was not registered");
            }
            IWindsorContainer container = containers[name];
            return IoC.UseLocalContainer(container);
        }
    }
}
