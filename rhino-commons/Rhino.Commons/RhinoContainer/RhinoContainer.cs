using System;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;

namespace Rhino.Commons
{
    public class RhinoContainer : WindsorContainer
    {
        private bool isDisposed = false;

        public RhinoContainer()
        {

        }

        public RhinoContainer(string fileName)
            : this(new XmlInterpreter(fileName))
        {
        }

        public RhinoContainer(IConfigurationInterpreter interpreter)
            : base()
        {
            DefaultConversionManager conversionManager = (DefaultConversionManager)Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            conversionManager.Add(new ConfigurationObjectConverter());

            interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
            RunInstaller();
        }

        public override T Resolve<T>(string key)
        {
            AssertNotDisposed();
            return base.Resolve<T>(key);
        }

        public override object Resolve(string key)
        {
            AssertNotDisposed();
            return base.Resolve(key);
        }

        public override object Resolve(string key, Type service)
        {
            AssertNotDisposed();
            return base.Resolve(key, service);
        }

        public override object Resolve(Type service)
        {
            AssertNotDisposed();
            return base.Resolve(service);
        }

        private void AssertNotDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The container has been disposed!");
        }

        public override void Dispose()
        {
            isDisposed = true;
            base.Dispose();
        }

        public bool IsDisposed
        {
            get { return isDisposed; }
        }
    }
}