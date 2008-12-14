using System.Messaging;
using Rhino.ServiceBus.Msmq;
using Xunit;
using System.Linq;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqSubscriptionTests : MsmqTestBase
    {
        [Fact]
        public void Can_read_subscription_from_queue()
        {
            transactionalQueue.Send(new Message
            {
                Label = TransactionalTestQueueUri.ToString(),
                Body = typeof(TestMessage).FullName
            },MessageQueueTransactionType.Single);

            var subscriptionStorage = new MsmqSubscriptionStorage(TransactionalTestQueueUri);

            var uri = subscriptionStorage
                .GetSubscriptionsFor(typeof(TestMessage))
                .Single();

            Assert.Equal(TransactionalTestQueueUri, uri);
        }

        public class TestMessage{}
    }
}