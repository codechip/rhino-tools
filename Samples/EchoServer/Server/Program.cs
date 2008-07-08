using System;
using System.Linq;
using log4net.Config;
using Rhino.Queues;
using Rhino.Queues.Cfg;

namespace Server
{
	class Program
	{
		static void Main()
		{
			BasicConfigurator.Configure();
			IQueueFactory factory = new Configuration("server")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueue("echo")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			factory.Start();
			
			Console.WriteLine("Starting to listen");
			using (var queue = factory.OpenQueue("echo"))
			{
				var message = queue.Recieve();
				var str = (string)message.Value;
				var rev = new string(str.ToCharArray().Reverse().ToArray());
				using(var remoteQueue = factory.OpenQueue(message.Source))
				{
					Console.WriteLine("Hanlded message");
					remoteQueue.Send(rev);
				}
			}
			Console.ReadLine();
		}
	}
}
