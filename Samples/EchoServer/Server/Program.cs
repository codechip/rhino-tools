using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Queues;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			IQueueFactory factory = new QueueConfiguration()
				.LocalUri("queue://localhost/server")
				.WorkerThreads(1)
				.PurgePendingMessages()
				.BuildQueueFactory();

			var queue = factory.GetLocalQueue("server");
			Console.WriteLine("Starting to listen");
			queue.MessageArrived += message =>
			{
				var str = Encoding.UTF8.GetString(message.Body);
				Console.WriteLine("Got message {0} with content {1} ", message.Id, str);
				var rev = new string(str.ToCharArray().Reverse().ToArray());
				IRemoteQueue remoteQueue = factory.GetRemoteQueue(message.Source);
				Console.WriteLine("Sending message {0} to {1}", rev, message.Source);
				remoteQueue.Send(new QueueMessage { Body = Encoding.UTF8.GetBytes(rev) });
			};
			Console.ReadLine();
		}
	}
}
