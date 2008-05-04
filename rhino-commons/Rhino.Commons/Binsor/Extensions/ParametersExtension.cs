namespace Rhino.Commons.Binsor.Extensions
{
    using System.Collections;
    using Castle.Core.Configuration;

    public class ParametersExtension : ConfigurationExtension
    {
        public ParametersExtension(IDictionary configuration)
            : base(configuration)
        {
        }

        protected override IConfiguration GetRootConfiguration(IConfiguration root)
        {
            IConfiguration parameters = root.Children["parameters"];

            if (parameters == null)
            {
                parameters = new MutableConfiguration("parameters");
                root.Children.Add(parameters);
            }

            return parameters;
        }
    }
}