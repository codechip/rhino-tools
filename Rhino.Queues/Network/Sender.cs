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
	using System.Diagnostics;

	public class Sender : ISender
	{
		private readonly ILog logger = LogManager.GetLogger(typeof(Sender));

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
				string endPoint;
				if (storage.WaitForNewMessages(TimeSpan.FromSeconds(1), out endPoint) == false)
				{
					foreach (var queue in storage.Queues)
					{
						SendMessagesFromEndpoint(queue);
					}
					continue;
				}
				if (endPoint == null)
					return;
				SendMessagesFromEndpoint(endPoint);
			}
		}

		private void SendMessagesFromEndpoint(string endPoint)
		{
			var array = storage.PullMessagesFor(endPoint, m => m.SendAt <= SystemTime.Now())
				.Take(100).ToArray();
			if (array.Length == 0)
			{
				NothingToSend();
				return;
			}
			try
			{
				logger.DebugFormat("Starting to send {0} messages to {1}", array.Length, endPoint);
				var request = (HttpWebRequest)WebRequest.Create(endPoint);
				request.Method = "PUT";
				using (var stream = request.GetRequestStream())
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
			catch (WebException e)
			{
				var response = ((HttpWebResponse)e.Response);

				if (response != null && response.StatusCode == HttpStatusCode.NotFound)
				{
					using (var sr = new StreamReader(response.GetResponseStream()))
					{
						NotFoundSendErrorHandling(endPoint, array, e, sr);
					}
				}
				else
				{
					DefaultSendErrorHandling(endPoint, array, e);
				}
			}
			catch (Exception e)
			{
				DefaultSendErrorHandling(endPoint, array, e);
			}
		}

		private void NotFoundSendErrorHandling(string endPoint, IEnumerable<TransportMessage> messages, Exception exception, StreamReader serverResponse)
		{
			logger.Warn("Failed to send messages to " + endPoint + " entering items to queue again", exception);
			string line;
			var failures = new Dictionary<Guid, MessageSendFailure>();
			while ((line = serverResponse.ReadLine()) != null)
			{
				try
				{
					var parts = line.Split(':');
					var sendFailures = (MessageSendFailure)Enum.Parse(typeof(MessageSendFailure), parts[1]);
					var id = new Guid(parts[0]);
					failures.Add(id, sendFailures);
				}
				catch (Exception e)
				{
					logger.Warn("failed to parse input from server error correctly, ignoring input line", e);
				}
			}
			foreach (var message in messages)
			{
				MessageSendFailure sendFailure;
				if (failures.TryGetValue(message.Id, out sendFailure) == false)
					sendFailure = MessageSendFailure.None;
				Error(exception, message, sendFailure);
			}
		}

		private void DefaultSendErrorHandling(string endPoint, IEnumerable<TransportMessage> messages, Exception e)
		{
			logger.Warn("Failed to send messages to " + endPoint + " entering items to queue again", e);
			foreach (var message in messages)
			{
				Error(e, message, MessageSendFailure.None);
			}
		}

		public event Action BatchSent = delegate { };
		public event Action NothingToSend = delegate { };
		public event Action<Exception, TransportMessage, MessageSendFailure> Error = delegate { };

		public void Dispose()
		{
			active = false;
			foreach (var thread in threads)
			{
				if (thread != Thread.CurrentThread)
					thread.Join();
			}
		}
	}
}