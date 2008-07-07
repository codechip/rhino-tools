using System;
using MbUnit.Framework;
using Rhino.Queues.Cfg;

namespace Rhino.Queues.Tests.Scenarios
{
	[TestFixture]
	public class Request_reply_scenario_tests
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
		public void Can_send_single_message()
		{
			serverFactory.Start();
			clientFactory.Start();

			serverFactory.Send("kong@client", new DateTime(2000, 1, 1));
			var actual = (DateTime)clientFactory.Queue("kong").Recieve();
			Assert.AreEqual(new DateTime(2000, 1, 1), actual);
		}

		[Test]
		public void Can_send_to_self()
		{
			serverFactory.Start();
			clientFactory.Start();

			clientFactory.Send("kong@client", new DateTime(2000, 1, 1));
			var actual = (DateTime)clientFactory.Queue("kong").Recieve();
			Assert.AreEqual(new DateTime(2000, 1, 1), actual);
		}

		[Test]
		public void Can_send_to_self_without_explicit_name()
		{
			serverFactory.Start();
			clientFactory.Start();

			clientFactory.Send("kong", new DateTime(2000, 1, 1));
			var actual = (DateTime)clientFactory.Queue("kong").Recieve();
			Assert.AreEqual(new DateTime(2000, 1, 1), actual);
		}


		[Test]
		public void Can_send_and_reply()
		{
			serverFactory.Start();
			clientFactory.Start();

			serverFactory.Send("kong@client", new DateTime(2000, 1, 1));
			var actual1 = (DateTime)clientFactory.Queue("kong").Recieve();
			Assert.AreEqual(new DateTime(2000, 1, 1), actual1);
			clientFactory.Send("foo@server", "passed all the way");
			var actual2 = (string)serverFactory.Queue("foo").Recieve();
			Assert.AreEqual("passed all the way", actual2);
			
		}
	}
}