using System.Threading;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Messages;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class BugWithLogTest : MsmqTestBase
    {
        private readonly IWindsorContainer container;

        public BugWithLogTest()
        {
            container = new WindsorContainer(new XmlInterpreter("BusWithLogging.config"));
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
        }

        [Fact]
        public void LoggingModule_should_be_in_container()
        {
            Assert.True(container.Kernel.HasComponent(typeof(MessageLoggingModule)));
        }

        [Fact]
        public void When_sending_message_will_place_copy_in_log_queue()
        {
            using(var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(new TestMessage());

                var message = testQueue2.Receive();
                var serializer = container.Resolve<IMessageSerializer>();
                var msg = (MessageArrivedMessage)serializer.Deserialize(message.BodyStream)[0];

                Assert.Equal(typeof(TestMessage).FullName, msg.MessageType);
            }
        }

        public class TestMessage
        {
            
        }

        public class TestHandler : ConsumerOf<TestMessage>
        {
            public void Consume(TestMessage message)
            {
                
            }
        }
    }
}