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
        private string bootStrapperName;

        public void SetBootStrapperTypeName(string typeName)
        {
            bootStrapperName = typeName;
        }

        public void Start<TBootStrapper>()
            where TBootStrapper : AbstractBootStrapper
        {
            SetBootStrapperTypeName(typeof (TBootStrapper).FullName);
            Start(typeof(TBootStrapper).Assembly.FullName);
        }

        public void Start(string asmName)
        {
            string logfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

            XmlConfigurator.ConfigureAndWatch(new FileInfo(logfile));

            assebmlyName = asmName;

            CreateBootStrapper();

            CreateContainer();

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
            var facility = new RhinoServiceBusFacility();
            bootStrapper.ConfigureBusFacility(facility);
            container.Kernel.AddFacility("rhino.esb", facility);
        }

       
        private void CreateBootStrapper()
        {
            logger.DebugFormat("Loading {0}", assebmlyName);
            var assembly = Assembly.Load(assebmlyName);

            Type bootStrapperType = null;

            if (string.IsNullOrEmpty(bootStrapperName) == false)
                bootStrapperType = assembly.GetType(bootStrapperName);
                
            bootStrapperType = bootStrapperType ?? 
                GetAutoBootStrapperType(assembly);
            try
            {
                bootStrapper = (AbstractBootStrapper)Activator.CreateInstance(bootStrapperType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Failed to create " + bootStrapperType + ".", e);
            }

        }

        public IWindsorContainer Container
        {
            get { return container; }
        }

        private static Type GetAutoBootStrapperType(Assembly assembly)
        {
            var bootStrappers = assembly.GetTypes()
                .Where(x => typeof (AbstractBootStrapper).IsAssignableFrom(x))
                .ToArray();
         
            if(bootStrappers.Length==0)
                throw new InvalidOperationException("Could not find a boot strapper for " + assembly);

            if (bootStrappers.Length == 2)
                throw new InvalidOperationException("Found more than one boot strapper for " + assembly +
                                                    " you need to specify which boot strapper to use");

            return bootStrappers[0];
        }

        public void Close()
        {
            if (serviceBus != null)
                serviceBus.Dispose();
        }

        public override object InitializeLifetimeService()
        {
            return null; //singleton
        }
    }
}