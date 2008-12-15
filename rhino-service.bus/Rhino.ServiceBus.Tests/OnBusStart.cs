using System.Threading;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Xunit;
using System.Linq;

namespace Rhino.ServiceBus.Tests
{
    public class OnBusStart : MsmqTestBase
    {
        private readonly IWindsorContainer container;

        public OnBusStart()
        {
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
            container.AddComponent<TestHandler>();
            container.AddComponent<OccasionalTestHandler>();
        }

        [Fact(Skip = "No longer relevant, need to rethink how to do this")]
        public void Should_subscribe_to_all_handlers_automatically()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                var messages = subscriptions.GetAllMessages();
                Assert.Equal("Add: " + bus.Endpoint, messages[0].Label);
            }
        }

        [Fact(Skip = "No longer relevant, need to rethink how to do this")]
        public void Would_not_automatically_subscribe_occasional_consumers()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                var messages = subscriptions.GetAllMessages();
                Assert.Equal(3, messages.Length);
            }
        }

        public class TestMessage { }
        public class AnotherTestMessage { }
        public class TestHandler : ConsumerOf<TestMessage>
        {
            public void Consume(TestMessage message)
            {
            }
        }

        public class OccasionalTestHandler : OccasionalConsumerOf<AnotherTestMessage>
        {
            public void Consume(AnotherTestMessage message)
            {
            }
        }
    }
}