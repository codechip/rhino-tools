using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using log4net.Config;
using Rhino.Queues;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException+=CurrentDomain_OnUnhandledException;
			Process.Start(@"..\..\..\Client\bin\debug\Client.exe"); 
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

		private static void CurrentDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.ExceptionObject.ToString());
		}
	}
}
