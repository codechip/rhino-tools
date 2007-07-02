using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using Rhino.Commons;

namespace PayRollSystem
{
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

        public ICollection<string> ContainerNames
        {
            get { return containers.Keys; }
        }

        public IDisposable Enter(string name)
        {
            if (containers.ContainsKey(name) == false)
            {
                throw new InvalidOperationException("Container " + name + " was not registered");
            }
            IWindsorContainer container = containers[name];
            return IoC.UseLocalContainer(container);
        }
    }
}
