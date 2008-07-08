using System;
using System.Text;
using log4net.Config;
using Rhino.Queues;
using Rhino.Queues.Cfg;

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
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			var queue = factory.OpenQueue("echo@server");
			Console.WriteLine("Starting to send...");
			var count = 100000;
			DateTime start = DateTime.Now;
			for (int i = 0; i < count; i++)
			{
				queue.Send("Msg #" + i);
			}
			TimeSpan duration = DateTime.Now - start;
			Console.WriteLine("Dispatched {0}  messages in {1}", count, duration);
			Console.ReadKey();
		}
	}
}
