using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor;
using Castle.Windsor.Configuration;

namespace Rhino.Commons
{
    public class RhinoContainer : WindsorContainer
    {
        public RhinoContainer(IConfigurationInterpreter interpreter) : base()
        {
            DefaultConversionManager conversionManager = (DefaultConversionManager) Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            conversionManager.Add(new ConfigurationObjectConverter());
         
            interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
            RunInstaller();
        }
    }
}