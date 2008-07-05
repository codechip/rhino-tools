using System;
using System.Text;
using log4net.Config;
using Rhino.Queues;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			BasicConfigurator.Configure();
			IQueueFactory factory = new QueueConfiguration()
				.LocalUri("queue://localhost/server")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();

			var queue = factory.GetLocalQueue("server");
			Console.WriteLine("Starting to listen");
			var count = 0;
			queue.MessageArrived += message =>
			{
				var str = Encoding.UTF8.GetString(message.Body);
				Console.WriteLine(str);
			};
			Console.ReadLine();
		}
	}
}
