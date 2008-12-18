using System.Linq;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqFlatQueueSubscriptionTests : MsmqFlatQueueTestBase
    {
        [Fact]
        public void Can_read_subscription_from_queue()
        {
            var serializer = new XmlMessageSerializer(new DefaultReflection());

            var msg = new Message();
            serializer.Serialize(new object[]{new AddSubscription
                                                  {
                                                      Endpoint = TransactionalTestQueueUri.ToString(),
                                                      Type = typeof(TestMessage).FullName,
                                                  }}, msg.BodyStream);

            queue.Send(msg, MessageQueueTransactionType.None);


            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                                                                  serializer,
                                                                  TestQueueUri,
                                                                  new FlatQueueStrategy(TestQueueUri));
            subscriptionStorage.Initialize();

            var uri = subscriptionStorage
                .GetSubscriptionsFor(typeof(TestMessage))
                .Single();

            Assert.Equal(TransactionalTestQueueUri, uri);
        }

        [Fact]
        public void Adding_then_removing_will_result_in_no_subscriptions()
        {
            var serializer = new XmlMessageSerializer(new DefaultReflection());
            var msg = new Message();
            serializer.Serialize(new object[]{new AddSubscription
                                                  {
                                                      Endpoint = TransactionalTestQueueUri.ToString(),
                                                      Type = typeof(TestMessage).FullName,
                                                  }}, msg.BodyStream);


            queue.Send(msg, MessageQueueTransactionType.None);

            var subscriptionStorage = new MsmqSubscriptionStorage(new DefaultReflection(),
                                                                  serializer,
                                                                  TestQueueUri,
                                                                  new FlatQueueStrategy(TestQueueUri));
            subscriptionStorage.Initialize();
            subscriptionStorage.RemoveSubscription(typeof(TestMessage).FullName, TransactionalTestQueueUri.ToString());

            var uris = subscriptionStorage
                .GetSubscriptionsFor(typeof(TestMessage));

            Assert.Equal(0, uris.Count());
        }

        public class TestMessage { }
    }
}
