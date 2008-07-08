using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using log4net;
using Rhino.Queues.Impl;
using Rhino.Queues.Storage;
using System.Linq;

namespace Rhino.Queues.Network
{
	public class Sender : ISender
	{
		private readonly ILog logger = LogManager.GetLogger(typeof (Sender));

		private readonly IMessageStorage storage;
		private readonly int workerThreadsCount;
		private readonly IList<Thread> threads = new List<Thread>();
		private bool active = true;

		public Sender(IMessageStorage storage, int workerThreadsCount)
		{
			this.storage = storage;
			this.workerThreadsCount = workerThreadsCount;
		}

		public void Start()
		{
			for (var i = 0; i < workerThreadsCount; i++)
			{
				var thread = new Thread(Send)
				{
					IsBackground = true
				};
				threads.Add(thread);
				thread.Start();
			}

		}

		private void Send()
		{
			while (active)
			{
				//TODO: Transaction
				var endPoint = storage.WaitForNewMessages();
				if(endPoint==null)
					return;
				var array = storage.PullMessagesFor(endPoint).Take(100).ToArray();
				if(array.Length==0)
					continue;
				try
				{
					logger.DebugFormat("Starting to send {0} messages to {1}", array.Length, endPoint);
					var request = (HttpWebRequest)WebRequest.Create(endPoint);
					request.Method = "PUT";
					using(var stream = request.GetRequestStream())
					{
						var stream1 = new MemoryStream();
						new BinaryFormatter().Serialize(stream1, array);
						stream1.Position = 0;
						var tmp = new BinaryFormatter().Deserialize(stream1);
						new BinaryFormatter().Serialize(stream, array);
					}
					request.GetResponse().Close();
					BatchSent();
				}
				catch (Exception e)
				{
					logger.Warn("Failed to send messages to " + endPoint +" entering items to queue again", e);
					foreach (var message in array)
					{
						storage.Add(endPoint, message);
					}
					Error(e, array);
				}
			}
		}

		public event Action BatchSent = delegate { };
		public event Action<Exception, TransportMessage[]> Error = delegate {};

		public void Dispose()
		{
			active = false;
			foreach (var thread in threads)
			{
				if(thread!=Thread.CurrentThread)
					thread.Join();
			}
		}
	}
}