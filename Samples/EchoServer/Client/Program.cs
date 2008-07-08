using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Queues;
using Rhino.Queues.Cfg;

namespace Client
{
	class Program
	{
		static void Main()
		{
			IQueueFactory factory = new Configuration("client")
				.Map("server").To("http://localhost:9999/server/")
				.Map("client").To("http://localhost:9999/client/")
				.RegisterQueue("echo-reply")
				.ListenerThreads(1)
				.SenderThreads(1)
				.BuildQueueFactory();

			using (var serverQueue = factory.OpenQueue("echo@server"))
			{
				Console.WriteLine("Sending 'hello there'");
				serverQueue.Send("Hello there");
			}
			using(var clientQueue = factory.OpenQueue("echo-reply"))
			{
				var msgText = clientQueue.Recieve();
				Console.WriteLine(msgText);
			}
		}
	}
}
