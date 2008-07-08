using System;
using System.Text;
using log4net.Config;
using Rhino.Queues;
using Rhino.Queues.Cfg;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			BasicConfigurator.Configure();
			IQueueFactory factory = new Configuration("server")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueue("echo")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			var queue = factory.OpenQueue("echo");
			Console.WriteLine("Starting to listen");
			while(true)
			{
				var msg = queue.Recieve();
				Console.WriteLine(msg);

			}
		}
	}
}
