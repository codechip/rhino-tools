using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Msmq;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class FlatQueueStrategyTests : MsmqFlatQueueTestBase
    {
        [Fact]
        public void Moving_to_errors_queue_removes_message_from_subscriptions_queue()
        {
            var queueStrategy = new FlatQueueStrategy(new EndpointRouter(),TestQueueUri.Uri);
            queue.Send(new TestMessage {Name = "ayende"});
            Message msg = queue.Peek(TimeSpan.FromSeconds(30));
            Assert.Equal(1, queue.GetCount());
            string msgId;
            queueStrategy.TryMoveMessage(queue, msg, SubQueue.Errors,out msgId);
            Assert.Equal(0, queue.GetCount());
        }

        [Fact]
        public void Moving_to_discarded_queue_removes_message_from_subscriptions_queue()
        {
            var queueStrategy = new FlatQueueStrategy(new EndpointRouter(), TestQueueUri.Uri);
            queue.Send(new TestMessage { Name = "ayende" });
            Message msg = queue.Peek(TimeSpan.FromSeconds(30));
            Assert.Equal(1, queue.GetCount());
            string msgId;
            queueStrategy.TryMoveMessage(queue, msg, SubQueue.Discarded, out msgId);
            Assert.Equal(0, queue.GetCount());
        }

        [Fact]
        public void Moving_to_subscription_queue_removes_message_from_root_queue()
        {
            var queueStrategy = new FlatQueueStrategy(new EndpointRouter(), TestQueueUri.Uri);
            queue.Send(new TestMessage { Name = "ayende" });
            Message msg = queue.Peek(TimeSpan.FromSeconds(30));
            Assert.Equal(1, queue.GetCount());
            string msgId;
            queueStrategy.TryMoveMessage(queue, msg, SubQueue.Subscriptions,out msgId);
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
