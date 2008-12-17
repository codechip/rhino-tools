using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using log4net;
using log4net.Config;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Hosting
{
    public class DefaultHost : MarshalByRefObject
    {
        private readonly ILog logger = LogManager.GetLogger(typeof (DefaultHost));
        private string assebmlyName;
        private AbstractBootStrapper bootStrapper;
        private IWindsorContainer container;
        private IStartableServiceBus serviceBus;

        public void Start(string asmName)
        {
            string logfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

            XmlConfigurator.ConfigureAndWatch(new FileInfo(logfile));

            assebmlyName = asmName;

            CreateContainer();

            CreateBootStrapper();

            InitializeContainer();

            bootStrapper.BeforeStart();

            logger.Debug("Starting bus");
            serviceBus = container.Resolve<IStartableServiceBus>();

            serviceBus.Start();

            bootStrapper.AfterStart();
        }

        private void InitializeContainer()
        {
            bootStrapper.InitializeContainer(container);
        }


        private void CreateContainer()
        {
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
        }

        private void CreateBootStrapper()
        {
            logger.DebugFormat("Loading {0}", assebmlyName);
            var assembly = Assembly.Load(assebmlyName);

            bootStrapper = assembly.GetTypes()
                .Where(x => typeof (AbstractBootStrapper).IsAssignableFrom(x))
                .Select(x => (AbstractBootStrapper) Activator.CreateInstance(x))
                .FirstOrDefault();

            if (bootStrapper == null)
                throw new InvalidOperationException("Could not find a boot strapper for " + assembly);
        }

        public void Close()
        {
            serviceBus.Dispose();
        }

        public override object InitializeLifetimeService()
        {
            return null; //singleton
        }
    }
}