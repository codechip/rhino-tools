using System.Collections.Generic;
using log4net.Config;
using Rhino.Queues.Cfg;
using MbUnit.Framework;

namespace Rhino.Queues.Tests.Scenarios
{
	[TestFixture]
	public class Sending_many_messages_scenario_tests
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
		public void Can_send_hundreds_of_messages()
		{
			BasicConfigurator.Configure();

			serverFactory.Start();
			clientFactory.Start();

			var iterationCount = 5000;

			for (int i = 0; i < iterationCount; i++)
			{
				serverFactory.OpenQueue("fu@client").Send(i);
				serverFactory.OpenQueue("kong@client").Send(i * 2);
			}
			var fu = new List<int>();
			var kong = new List<int>();
			for (int i = 0; i < iterationCount; i++)
			{
				fu.Add((int)clientFactory.OpenQueue("fu").Recieve().Value);
				kong.Add((int)clientFactory.OpenQueue("kong").Recieve().Value);
			}

			// we do not ensure order
			fu.Sort();
			kong.Sort();

			Assert.AreEqual(iterationCount, fu.Count);
			Assert.AreEqual(iterationCount, kong.Count);

			for (int i = 0; i < iterationCount; i++)
			{
				Assert.AreEqual(i, fu[i]);
				Assert.AreEqual(i*2, kong[i]);
			}
		}
	}
}