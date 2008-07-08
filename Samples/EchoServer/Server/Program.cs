using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Queues;
using Rhino.Queues.Cfg;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			IQueueFactory factory = new Configuration("server")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueue("echo")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			
			Console.WriteLine("Starting to listen");
			using (var queue = factory.OpenQueue("server"))
			{
				var str = (string)queue.Recieve().Value;
				var rev = new string(str.ToCharArray().Reverse().ToArray());
				using(var remoteQueue = factory.OpenQueue(null))
				{
					remoteQueue.Send(rev);
				}
			}
			Console.ReadLine();
		}
	}
}
