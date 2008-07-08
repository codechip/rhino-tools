using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using log4net.Config;
using Rhino.Queues;
using Rhino.Queues.Cfg;
using Rhino.Queues.Impl;

namespace Client
{
	class Program
	{
		static void Main()
		{
			BasicConfigurator.Configure();
			IQueueFactory factory = new Configuration("client")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueue("echo-reply")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			factory.Start();

			using (var tx = new TransactionScope())
			using (var serverQueue = factory.OpenQueue("echo@server"))
			{
				Console.WriteLine("Sending 'hello there'");
				serverQueue.Send("Hello there").Source = new Destination("echo-reply@client");
				tx.Complete();
			}
			using (var clientQueue = factory.OpenQueue("echo-reply"))
			{
				var msgText = clientQueue.Recieve();
				Console.WriteLine(msgText.Value);
			}
		}
	}
}
