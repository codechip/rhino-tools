using System;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Model;
using Castle.Model.Configuration;

namespace Rhino.Commons
{
    public class NestedConfigurationFacility : IFacility, ISubDependencyResolver
    {
        private IKernel kernel;
        private IConfiguration configuration;

        public void Init(IKernel theKernel, IConfiguration facilityConfig)
        {
            theKernel.Resolver.AddSubResolver(this);
            kernel = theKernel;
            configuration = facilityConfig.Children["configuration"];
            if (configuration == null)
            {
                throw new InvalidOperationException(
                    "Nested Confiugration Facility must have a <configuration> element inside the <facility ...> element");
            }
        }

        public object Resolve(CreationContext context, ComponentModel model, DependencyModel dependency)
        {
            string key = GetKey(dependency, model);
            string reference = key.Substring(2, key.Length - 3);
            string[] items = reference.Split(new string[] { "::" }, StringSplitOptions.None);
            AssertHasBothPartsOfString(items, reference);
            string propertyKey = items[0].Trim();
            string propertyPath = items[1].Trim();
            IConfiguration child = configuration.Children[propertyKey];
            if (child == null)
            {
                throw new InvalidOperationException(
                    string.Format("Could not find nested configuration {0} for {1}", propertyKey,
                                  dependency.DependencyKey));
            }
            return ConvertToRealValue(child, dependency, propertyPath);
        }

        private object ConvertToRealValue(IConfiguration child, DependencyModel dependency, string propertyPath)
        {
            IConversionManager conversionManager = (IConversionManager)kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
            if (child.Children[propertyPath] != null)
            {
                return conversionManager.PerformConversion(child.Children[propertyPath], dependency.TargetType);
            }
            else
            {
                return conversionManager.PerformConversion(child.Attributes[propertyPath], dependency.TargetType);
            }
        }

        private static void AssertHasBothPartsOfString(string[] items, string realKey)
        {
            if (items.Length != 2)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Nested configuration keys should be in the following format @{{ key :: value}}, but was {0}",
                        realKey));
            }
        }

        public bool CanResolve(CreationContext context, ComponentModel model, DependencyModel dependency)
        {
            string key = GetKey(dependency, model);
            if (key == null || key.Length <= 3 ||
                !key.StartsWith("@{") || !key.EndsWith("}"))
            {
                return false;
            }
            return true;
        }

        private static string GetKey(DependencyModel dependency, ComponentModel model)
        {
            ParameterModel parameter = model.Parameters[dependency.DependencyKey];
            if (parameter == null)
                return null;
            return parameter.Value;
        }

        public void Terminate()
        {
        }
    }


}