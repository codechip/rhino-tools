using Castle.Core.Configuration;
using Castle.MicroKernel;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Configuration
{
    public interface IBusConfigurationAware
    {
        void Configure(
            RhinoServiceBusFacility facility, 
            IConfiguration configuration
            );
    }
}