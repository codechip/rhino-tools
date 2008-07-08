using System;
using System.Transactions;
using Rhino.Queues.Cfg;
using MbUnit.Framework;

namespace Rhino.Queues.Tests.Scenarios
{
	[TestFixture]
	public class Can_handle_batch_where_some_messages_are_not_valid
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
		public void When_sending_messages_to_existing_queue_and_non_existing_queue_messages_to_existing_queue_will_arrive()
		{
			serverFactory.Start();
			clientFactory.Start();

			using(var tx = new TransactionScope())
			using (var queue = serverFactory.OpenQueue("kong@client"))
			{
				queue.Send(new DateTime(2000, 1, 1));
				queue.Send(new DateTime(2000, 1, 1));
				tx.Complete();
			}
			var actual = (DateTime)clientFactory.OpenQueue("kong").Recieve().Value;
			Assert.AreEqual(new DateTime(2000, 1, 1), actual);
		}
	}
}