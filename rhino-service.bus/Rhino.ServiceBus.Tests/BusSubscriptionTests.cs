using System;
using System.Messaging;
using System.Threading;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class BusSubscriptionTests : MsmqTestBase
    {
        private readonly IWindsorContainer container;

        public BusSubscriptionTests()
        {
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
        }

        [Fact]
        public void Adding_then_removing_will_result_in_compacted_queue()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                var storage = container.Resolve<ISubscriptionStorage>();
                var wait = new ManualResetEvent(false);
                int count = 0;
                storage.SubscriptionChanged += () =>
                {
                    if (Interlocked.Increment(ref count) == 2)
                        wait.Set();
                };

                bus.Subscribe<MsmqSubscriptionTests.TestMessage>();
                bus.Subscribe<OnBusStart.TestMessage>();

                wait.WaitOne();

                storage.RemoveSubscription(typeof(MsmqSubscriptionTests.TestMessage).FullName, TestQueueUri.ToString());

                count = 0;
                var enumerator2 = subscriptions.GetMessageEnumerator2();
                while (enumerator2.MoveNext(TimeSpan.FromSeconds(0)))
                    count += 1;
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public void Can_add_subscription_to_queue()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();
                var storage = container.Resolve<ISubscriptionStorage>();
                var wait = new ManualResetEvent(false);
                storage.SubscriptionChanged += () =>
                {
                    wait.Set();
                };

                bus.Subscribe<OnBusStart.TestMessage>();

                wait.WaitOne();

                var peek = subscriptions.Peek();

                Assert.NotNull(peek);
            }
        }



        [Fact]
        public void Can_remove_subscription_from_queue()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                var storage = container.Resolve<ISubscriptionStorage>();
                var wait = new ManualResetEvent(false);
                int count = 0;
                storage.SubscriptionChanged += () =>
                {
                    if (Interlocked.Increment(ref count) == 2)
                        wait.Set();
                };

                bus.Subscribe<MsmqSubscriptionTests.TestMessage>();
                bus.Unsubscribe<MsmqSubscriptionTests.TestMessage>();
                
                wait.WaitOne();
                try
                {
                    subscriptions.Peek(TimeSpan.FromSeconds(0));
                    Assert.False(true);
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                        throw;
                }
            }
        }
    }
}