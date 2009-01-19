using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Xunit;

namespace Rhino.ServiceBus.Tests.Dht
{
    public class UsageErrors
    {
        [Fact]
        public void cannot_create_saga_for_dht_without_consuming_merge_saga_state()
        {
            var container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb",
                new RhinoServiceBusFacility()
                    .UseDhtSagaPersister()
                );
            var exception = Assert.Throws<InvalidUsageException>(
                () => container.AddComponent<InvalidBaristaSaga>());
            Assert.Equal(
                "Rhino.ServiceBus.Tests.Dht.InvalidBaristaSaga must implement Orchestrates<MergeSagaState>, to handle state merges.", 
                exception.Message);
        }
    }
}