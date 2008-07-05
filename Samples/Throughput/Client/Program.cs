using System;
using System.Text;
using log4net.Config;
using Rhino.Queues;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			BasicConfigurator.Configure(); 
			IQueueFactory factory = new QueueConfiguration()
				.LocalUri("queue://localhost/client")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();

			IRemoteQueue queue = factory.GetRemoteQueue(new Uri("queue://localhost/server"));
			Console.WriteLine("Sending hello there");
			var count = 100000;
			DateTime start = DateTime.Now;
			for (int i = 0; i < count; i++)
			{
				queue.Send(new QueueMessage { Body = Encoding.UTF8.GetBytes("Msg #" + i) });
			}
			TimeSpan duration = DateTime.Now - start;
			Console.ForegroundColor = ConsoleColor.Red;
			
			Console.WriteLine("Dispatched {0}  messages in {1}", count, duration);
			Console.ReadKey();
		}
	}
}
