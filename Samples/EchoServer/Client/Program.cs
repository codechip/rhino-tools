using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Queues;

namespace Client
{
	class Program
	{
		static void Main()
		{
			IQueueFactory factory = new QueueConfiguration()
				.LocalUri("queue://localhost/client")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();

			IRemoteQueue queue = factory.GetRemoteQueue(new Uri("queue://localhost/server"));
			Console.WriteLine("Sending hello there");
			queue.Send(new QueueMessage {Body = Encoding.UTF8.GetBytes("Hello there")});
			QueueMessage message = factory.GetLocalQueue("client").Recieve();
			if(message==null)
			{
				Console.WriteLine("Did not get reply message in 1 minute!");
				return;
			}
			string msgText = Encoding.UTF8.GetString(message.Body);
			Console.WriteLine(msgText);
		}
	}
}
