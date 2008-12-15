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

        [Fact(Skip = "Not relevant anymore, should move to higher integration")]
        public void Adding_then_removing_will_result_in_compacted_queue()
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


            var storage = new MsmqSubscriptionStorage(new DefaultReflection(),
                serializer, 
                SubscriptionsUri);
            storage.RemoveSubscription(typeof(TestMessage).FullName, TestQueueUri.ToString());

            int count = 0;
            var enumerator2 = subscriptions.GetMessageEnumerator2();
            while (enumerator2.MoveNext(TimeSpan.FromSeconds(0)))
                count += 1;
            Assert.Equal(1, count);
        }

        [Fact(Skip = "Not relevant anymore, should move to higher integration")]
        public void Can_add_subscription_to_queue()
        {
            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                new JsonSerializer(new DefaultReflection()), 
                SubscriptionsUri);
            subscriptionStorage.AddSubscription(typeof (TestMessage).FullName, TestQueueUri.ToString());
            subscriptions.Formatter = new XmlMessageFormatter(new[] {typeof (string)});

            var peek = subscriptions.Peek();
            Assert.NotNull(peek);
            Assert.Equal("Add: msmq://./test_queue", peek.Label);
            Assert.Equal(typeof(TestMessage).FullName, peek.Body);
        }

        [Fact(Skip = "Not relevant anymore, should move to higher integration")]
        public void Can_remove_subscription_from_queue()
        {
            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                new JsonSerializer(new DefaultReflection()), 
                SubscriptionsUri);
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