using System;
using System.IO;
using System.Messaging;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Msmq;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
	public class QueueCreationModuleTests : IDisposable
	{
		private readonly Endpoint endPoint = new Uri("msmq://localhost/init_test").ToEndpoint();
		private WindsorContainer container;

		public QueueCreationModuleTests()
		{
			CleanQueue();
		}

		#region IDisposable Members

		public void Dispose()
		{
			CleanQueue();
		}

		#endregion

		[Fact]
		public void Should_create_subqueue_strategy_queues()
		{
			container = new WindsorContainer(new XmlInterpreter(
			                                 	Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InitBus.config")
			                                 	));
			container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility().UseSubqueuesQueueStructure());
			using (var bus = container.Resolve<IStartableServiceBus>())
			{
				bus.Start();
				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)));
			}
		}

		[Fact]
		public void Should_create_flat_queue_strategy_queues()
		{
			container = new WindsorContainer(new XmlInterpreter(
			                                 	Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InitBus.config")
			                                 	));
			container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility().UseFlatQueueStructure());
			using (var bus = container.Resolve<IStartableServiceBus>())
			{
				bus.Start();

				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)));
				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#subscriptions"));
				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#errors"));
				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#discarded"));
				Assert.True(MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#timeout"));
			}
			CleanQueue();
		}

		private void CleanQueue()
		{
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint)))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint));
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#subscriptions"))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint) + "#subscriptions");
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#errors"))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint) + "#errors");
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#discarded"))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint) + "#discarded");
			}
			if (MessageQueue.Exists(MsmqUtil.GetQueuePath(endPoint) + "#timeout"))
			{
				MessageQueue.Delete(MsmqUtil.GetQueuePath(endPoint) + "#timeout");
			}
		}
	}
}