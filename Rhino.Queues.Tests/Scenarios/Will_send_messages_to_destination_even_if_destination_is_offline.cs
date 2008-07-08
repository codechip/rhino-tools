using System.Threading;
using MbUnit.Framework;
using Rhino.Queues.Cfg;

namespace Rhino.Queues.Tests.Scenarios
{
	[TestFixture]
	public class Will_send_messages_to_destination_even_if_destination_is_offline
	{
		private IQueueFactory serverFactory;
		private IQueueFactory clientFactory;

		[SetUp]
		public void SetUp()
		{
			serverFactory = new Configuration("server")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueues("foo", "bar")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			clientFactory = new Configuration("client")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueues("kong", "fu")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();
		}

		[TearDown]
		public void TearDown()
		{
			serverFactory.Dispose();
			clientFactory.Dispose();
		}

		[Test]
		public void Server_is_started_after_client()
		{
			clientFactory.Start();
			var reset = new ManualResetEvent(false);
			clientFactory.DeliveryFailure += (message, exception) => reset.Set();
			clientFactory.OpenQueue("foo@server").Send(1);

			reset.WaitOne();
			serverFactory.Start();
			var value = serverFactory.OpenQueue("foo").Recieve().Value;
			Assert.AreEqual(1, value);
		}
	}
}