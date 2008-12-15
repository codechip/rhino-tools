using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using System.Linq;
using Rhino.ServiceBus.Serializers;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqSubscriptionTests : MsmqTestBase
    {
        [Fact]
        public void Can_read_subscription_from_queue()
        {
            var serializer = new JsonSerializer(new DefaultReflection());

            var msg = new Message();
            serializer.Serialize(new object[]{new AddSubscription
            {
                Endpoint = TransactionalTestQueueUri.ToString(),
                Type = typeof(TestMessage).FullName,
            }}, new MsmqTransportMessage(msg));

            queue.Send(msg, MessageQueueTransactionType.None);
            msg = queue.Peek();
            queue.MoveToSubQueue("subscriptions", msg);


            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                serializer,
                SubscriptionsUri);

            var uri = subscriptionStorage
                .GetSubscriptionsFor(typeof(TestMessage))
                .Single();

            Assert.Equal(TransactionalTestQueueUri, uri);
        }

        [Fact]
        public void Adding_then_removing_will_result_in_no_subscriptions()
        {
            var serializer = new JsonSerializer(new DefaultReflection());
            var msg = new Message();
            serializer.Serialize(new object[]{new AddSubscription
            {
                Endpoint = TransactionalTestQueueUri.ToString(),
                Type = typeof(TestMessage).FullName,
            }}, new MsmqTransportMessage(msg));


            queue.Send(msg, MessageQueueTransactionType.None);
            msg = queue.Peek();
            queue.MoveToSubQueue("subscriptions",msg);

            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                serializer, 
                SubscriptionsUri);
            subscriptionStorage.RemoveSubscription(typeof(TestMessage).FullName, TransactionalTestQueueUri.ToString());

            var uris = subscriptionStorage
                .GetSubscriptionsFor(typeof (TestMessage));

            Assert.Equal(0, uris.Count());
        }

        public class TestMessage{}
    }
}