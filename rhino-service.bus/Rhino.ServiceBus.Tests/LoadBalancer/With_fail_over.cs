using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer;
using Rhino.ServiceBus.Messages;
using Xunit;

namespace Rhino.ServiceBus.Tests.LoadBalancer
{
    public class With_fail_over : LoadBalancingTestBase
    {
        private readonly IWindsorContainer container;
        
        public With_fail_over()
        {
            var interpreter = new XmlInterpreter(@"LoadBalancer\BusWithLoadBalancer.config");
            container = new WindsorContainer(interpreter);
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());

            container.Register(
                Component.For<MsmqLoadBalancer>()
                    .DependsOn(new
                    {
                        threadCount = 1,
                        endpoint = new Uri(loadBalancerQueue),
                        SecondaryLoadBalancer = TestQueueUri2.Uri
                    })
                );
        }  
 
        [Fact]
        public void when_start_load_balancer_that_has_secondary_will_start_sending_heartbeats_to_secondary()
        {
            using(var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                loadBalancer.Start();

                var peek = testQueue2.Peek();
                var msgs = container.Resolve<IMessageSerializer>().Deserialize(peek.BodyStream);

                Assert.IsType<HeartBeat>(msgs[0]);
                var beat = (HeartBeat) msgs[0];
                Assert.Equal(loadBalancer.Endpoint.Uri, beat.From);
            }
            
        }

        [Fact]
        public void Will_send_heartbeat_every_second()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                loadBalancer.Start();

                var messageSerializer = container.Resolve<IMessageSerializer>();

                var msg1 = testQueue2.Receive();
                var beat1 = (HeartBeat)messageSerializer.Deserialize(msg1.BodyStream)[0];

                var msg2 = testQueue2.Receive();
                var beat2 = (HeartBeat)messageSerializer.Deserialize(msg2.BodyStream)[0];

                Assert.InRange((beat2.At - beat1.At).TotalSeconds, 1, 2);
            }
        }
    }
}