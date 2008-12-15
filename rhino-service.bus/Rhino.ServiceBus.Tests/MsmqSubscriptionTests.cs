using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Msmq;
using System.Linq;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqSubscriptionTests : MsmqTestBase
    {
        [Fact]
        public void Can_read_subscription_from_queue()
        {
            subscriptions.Send(new Message
            {
                Label = "Add: " + TransactionalTestQueueUri,
                Body = typeof(TestMessage).FullName
            },MessageQueueTransactionType.Single);

            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),SubscriptionsUri);

            var uri = subscriptionStorage
                .GetSubscriptionsFor(typeof(TestMessage))
                .Single();

            Assert.Equal(TransactionalTestQueueUri, uri);
        }

        [Fact]
        public void Adding_then_removing_will_result_in_no_subscriptions()
        {
            subscriptions.Send(new Message
            {
                Label = "Add: " + TransactionalTestQueueUri,
                Body = typeof(TestMessage).FullName
            }, MessageQueueTransactionType.Single);

            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(), SubscriptionsUri);
            subscriptionStorage.RemoveSubscription(typeof(TestMessage).FullName, TransactionalTestQueueUri.ToString());

            var uris = subscriptionStorage
                .GetSubscriptionsFor(typeof (TestMessage));

            Assert.Equal(0, uris.Count());
        }

        [Fact]
        public void Adding_then_removing_will_result_in_compacted_queue()
        {
            subscriptions.Send(new Message
            {
                Label = "Add: " + TransactionalTestQueueUri,
                Body = typeof(TestMessage).FullName
            }, MessageQueueTransactionType.Single);

            subscriptions.Send(new Message
            {
                Label = "Add: " + TestQueueUri,
                Body = typeof(TestMessage).FullName
            }, MessageQueueTransactionType.Single);

            var storage = new MsmqSubscriptionStorage(new DefaultReflection(), SubscriptionsUri);
            storage.RemoveSubscription(typeof(TestMessage).FullName, TestQueueUri.ToString());

            int count = 0;
            var enumerator2 = subscriptions.GetMessageEnumerator2();
            while (enumerator2.MoveNext(TimeSpan.FromSeconds(0)))
                count += 1;
            Assert.Equal(1, count);
        }

        [Fact]
        public void Can_add_subscription_to_queue()
        {
            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(), SubscriptionsUri);
            subscriptionStorage.AddSubscription(typeof (TestMessage).FullName, TestQueueUri.ToString());
            subscriptions.Formatter = new XmlMessageFormatter(new[] {typeof (string)});

            var peek = subscriptions.Peek();
            Assert.NotNull(peek);
            Assert.Equal("Add: msmq://./test_queue", peek.Label);
            Assert.Equal(typeof(TestMessage).FullName, peek.Body);
        }

        [Fact]
        public void Can_remove_subscription_from_queue()
        {
            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(), SubscriptionsUri);
            subscriptionStorage.AddSubscription(typeof(TestMessage).FullName, TestQueueUri.ToString());
            subscriptionStorage.RemoveSubscription(typeof(TestMessage).FullName, TestQueueUri.ToString());
            subscriptions.Formatter = new XmlMessageFormatter(new[] { typeof(string) });

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

        public class TestMessage{}
    }
}