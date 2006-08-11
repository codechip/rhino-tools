using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class NestedConfigurationFacilityTests
    {
        [Test]
        public void CanUseSubResolverToGetNestedItems()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter("NestedItems.config"));
            DemoClass demo = container.Resolve<DemoClass>();
            Assert.AreEqual("baz", demo.Name);
        }
    }

    public class DemoClass
    {
        private string name;

        public string Name
        {
            get { return name; }
        }

        public DemoClass(string name)
        {
            this.name = name;
        }
    }
}
