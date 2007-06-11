using System;
using System.IO;
using Castle.Core.Configuration;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;

namespace Rhino.Commons.Test.Binsor
{
    [TestFixture]
    public class BinsorFacilities_TestCase
    {
        private IWindsorContainer _container;

        [SetUp]
        public void TestInitialize()
        {
            _container = new RhinoContainer();
            string path =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Binsor\Facilities.boo"));
            Console.WriteLine(path);
            BooReader.Read(_container, path);
        }

        [Test]
        public void CanUseFacilities()
        {
            IFacility[] facilities = _container.Kernel.GetFacilities();
            Assert.AreNotEqual(0, facilities.Length);
        }

        [Test]
        public void CanAddConfiguarion()
        {
            IFacility[] facilities = _container.Kernel.GetFacilities();
            LoggingFacility logging = (LoggingFacility)Array.Find(facilities,
                           delegate(IFacility obj)
                               {
                                   return obj is LoggingFacility;
                               });
            string attribute = logging.FacilityConfig.Attributes["loggingApi"];
            Assert.AreEqual("Log4net", attribute);
            attribute = logging.FacilityConfig.Attributes["configFile"];
            Assert.AreEqual("log4net.config", attribute);
        }

        [Test]
        public void NestedConfiguration()
        {
            IFacility[] facilities = _container.Kernel.GetFacilities();
            LoggingFacility logging = (LoggingFacility)Array.Find(facilities,
                           delegate(IFacility obj)
                           {
                               return obj is LoggingFacility;
                           });

            IConfiguration child = logging.FacilityConfig.Children["NestedConfig"];
            Assert.IsNotNull(child);
            string attribute = child.Attributes["something"];
            Assert.AreEqual("foo", attribute);
            attribute = child.Attributes["bar"];
            Assert.AreEqual("nar", attribute);
        }
    }
}