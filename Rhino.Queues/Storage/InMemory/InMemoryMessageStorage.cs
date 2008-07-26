using System;
using System.Collections.Generic;
using System.Threading;
using Rhino.Queues.Impl;
using Rhino.Queues.Threading;

namespace Rhino.Queues.Storage.InMemory
{
	public class InMemoryMessageStorage : IMessageStorage
	{
		readonly IBlockingQueue<string> messagesEvents = new BlockingQueue<string>();
		private readonly IDictionary<string, QueuePackage> queuesByName =
			new Dictionary<string, QueuePackage>(StringComparer.InvariantCultureIgnoreCase);


		public InMemoryMessageStorage(IEnumerable<string> queues)
		{
			foreach (var endpoint in new HashSet<string>(queues))
			{
				queuesByName.Add(endpoint, new QueuePackage());
			}
		}
		public IEnumerable<TransportMessage> PullMessagesFor(string name, Predicate<TransportMessage> predicate)
		{
			var queue = GetQueue(name);
			lock (queue)
			{
				var messages = queue.MessagesInReverseOrder;
				foreach (var message in messages)
				{
					if(predicate(message)==false)
						continue;
					messages.RemoveCurrent();
					yield return message;
				}
			}
		}
		public IEnumerable<TransportMessage> PullMessagesFor(string name)
		{
			return PullMessagesFor(name, x => true);
		}

		public bool WaitForNewMessages(string name)
		{
			return GetQueue(name).WaitForNewMessage();
		}

		public bool Exists(string name)
		{
			return queuesByName.ContainsKey(name);
		}

		public bool WaitForNewMessages(TimeSpan timeToWait, out string queueWithNewMessages)
		{
			return messagesEvents.Dequeue(timeToWait, out queueWithNewMessages);
		}

		public IEnumerable<string> Queues
		{
			get
			{
				return queuesByName.Keys;
			}
		}

		public void MarkMessagesAsSent(TransportMessage[] array)
		{
			// we just ignore this, because to mark them as sent means just
			// removing them from memory
		}

		public void Add(string name, TransportMessage message)
		{
			QueuePackage queue = GetQueue(name);
			lock (queue)
			{
				queue.Add(message);
				messagesEvents.Enqueue(name);
			}
		}

		private QueuePackage GetQueue(string name)
		{
			QueuePackage queue;
			if (queuesByName.TryGetValue(name, out queue) == false)
				throw new ArgumentException("Queue '" + name + "' was not registered");
			return queue;
		}

		public void Dispose()
		{
			messagesEvents.Dispose();
		}
	}
}