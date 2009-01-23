using System;
using System.Messaging;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Msmq.TransportActions;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
	public class QueueInitializationModuleTests : IDisposable
	{
		readonly Uri endPoint = new Uri("msmq://localhost/init_test");

		public QueueInitializationModuleTests()
		{
			CleanQueue();
		}
		[Fact]
		public void Should_initialize_subqueue_strategy_queues()
		{
			
			var transport = new MsmqTransport(null, endPoint, 1, new IMessageAction[0]);
			var sut = new QueueInitializationModule(new SubQueueStrategy());
			sut.Init(transport);
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)));
			
		}
		[Fact]
		public void Should_initialize_flat_queue_strategy_queues()
		{
			var transport = new MsmqTransport(null, endPoint, 1, new IMessageAction[0]);
			var sut = new QueueInitializationModule(new FlatQueueStrategy(endPoint));
			sut.Init(transport);
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)));
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#subscriptions")));
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#errors")));
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#discarded")));
			Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#timeout")));

		}

		private void CleanQueue()
		{
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint));
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#subscriptions")))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint + "#subscriptions"));
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#errors")))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint + "#errors"));
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#discarded")))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint + "#discarded"));
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint + "#timeout")))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint + "#timeout"));
			}
		}
		public void Dispose()
		{
			CleanQueue();			
		}
	}
}