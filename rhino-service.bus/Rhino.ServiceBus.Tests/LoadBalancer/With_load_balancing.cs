using System;
using System.Messaging;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.LoadBalancer;
using Rhino.ServiceBus.Messages;
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

        [Fact(Skip = "broke load balancer while working on it")]
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

        [Fact(Skip = "broke load balancer while working on it")]
        public void Will_send_administrative_messages_to_all_nodes()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                loadBalancer.Start();
                bus.Start();

                bus.Send(loadBalancer.Endpoint, new ReadyToWork
                {
                    Endpoint = TransactionalTestQueueUri.Uri
                });

                bus.Send(loadBalancer.Endpoint, new ReadyToWork
                {
                    Endpoint = TestQueueUri2.Uri
                });

                bus.Send(loadBalancer.Endpoint, new AddSubscription
                {
                    Endpoint = bus.Endpoint.Uri.ToString(),
                    Type = "foobar"
                });
            }

            using(var q = new MessageQueue(MsmqUtil.GetQueuePath(TransactionalTestQueueUri)))
            {
                var message = q.Receive(MessageQueueTransactionType.Single);
                Assert.Equal("Rhino.ServiceBus.Messages.AddSubscription", message.Label);
            }

            using (var q = new MessageQueue(MsmqUtil.GetQueuePath(TestQueueUri2)))
            {
                var message = q.Receive(MessageQueueTransactionType.Single);
                Assert.Equal("Rhino.ServiceBus.Messages.AddSubscription", message.Label);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            using (var q = new MessageQueue(MsmqUtil.GetQueuePath(new Uri(loadBalancerQueue).ToEndpoint())))
            {
                q.Purge();
            }
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