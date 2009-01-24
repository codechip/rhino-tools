using System;
using System.Messaging;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.LoadBalancer.Msmq;
using Rhino.ServiceBus.Msmq;
using Xunit;

namespace Rhino.ServiceBus.Tests.LoadBalancer
{
    public class With_load_balancing : MsmqTestBase
    {
        private readonly IWindsorContainer container;

        private const string loadBalancerQueue = "msmq://localhost/test_queue.balancer";

        public With_load_balancing()
        {
            var interpreter = new XmlInterpreter(@"LoadBalancer\BusWithLoadBalancer.config");
            container = new WindsorContainer(interpreter);
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());

            container.AddComponent<MyHandler>();

            container.Register(
                Component.For<MsmqLoadBalancer>()
                    .DependsOn(new
                    {
                        threadCount = 1,
                        endpoint = new Uri(loadBalancerQueue)
                    })
                );
        }

        [Fact]
        public void Can_send_message_through_load_balancer()
        {
            MyHandler.ResetEvent = new ManualResetEvent(false);

            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                loadBalancer.Start();
                bus.Start();

                bus.Send(loadBalancer.Endpoint, "abcdefg");

                MyHandler.ResetEvent.WaitOne();
                Assert.True(
                    MyHandler.Message.ResponseQueue.Path.Contains(@"private$\test_queue")
                    );

                Assert.Equal("abcdefg", MyHandler.Value);
            }

        }

        public override void Dispose()
        {
            base.Dispose();
            MessageQueue.Delete(MsmqUtil.GetQueuePath(loadBalancerQueue));
        }

        public class MyHandler : ConsumerOf<string>
        {
            public static ManualResetEvent ResetEvent;
            public static string Value;
            public static Message Message;

            public void Consume(string message)
            {
                Message = MsmqTransport.CurrentMessageInformation.MsmqMessage;
                Value = message;
                ResetEvent.Set();
            }

        }
    }
}