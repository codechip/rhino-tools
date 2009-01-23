using System.Messaging;
using Rhino.ServiceBus.Msmq;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class FlatQueueStrategyTests : MsmqFlatQueueTestBase
    {
        [Fact]
        public void Moving_to_errors_queue_removes_message_from_subscriptions_queue()
        {
            var queueStrategy = new FlatQueueStrategy(TestQueueUri);
            queue.Send(new TestMessage {Name = "ayende"});
            Message msg = queue.Peek();
            Assert.Equal(1, queue.GetCount());
            queueStrategy.MoveToErrorsQueue(queue, msg);
            Assert.Equal(0, queue.GetCount());
        }

        [Fact]
        public void Moving_to_discarded_queue_removes_message_from_subscriptions_queue()
        {
            var queueStrategy = new FlatQueueStrategy(TestQueueUri);
            queue.Send(new TestMessage {Name = "ayende"});
            Message msg = queue.Peek();
            Assert.Equal(1, queue.GetCount());
            queueStrategy.MoveToDiscardedQueue(queue, msg);
            Assert.Equal(0, queue.GetCount());
        }

        [Fact]
        public void Moving_to_subscription_queue_removes_message_from_root_queue()
        {
            var queueStrategy = new FlatQueueStrategy(TestQueueUri);
            queue.Send(new TestMessage {Name = "ayende"});
            Message msg = queue.Peek();
            Assert.Equal(1, queue.GetCount());
            queueStrategy.MoveToSubscriptionQueue(queue, msg);
            Assert.Equal(0, queue.GetCount());
        }

        #region Nested type: TestMessage

        public class TestMessage
        {
            public string Name { get; set; }
        }

        #endregion
    }
}