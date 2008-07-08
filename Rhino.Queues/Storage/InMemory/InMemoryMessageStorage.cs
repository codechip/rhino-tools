using System;
using System.Collections.Generic;
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
				var messages = queue.Messages;
				var current = messages.Last;
				while (current != null) 
				{
					var value = current.Value;
					var toRemove = current;
					current = current.Previous;
					if (predicate(value) == false) 
						continue;
					messages.Remove(toRemove);
					yield return value;
				}
			}
		}
		public IEnumerable<TransportMessage> PullMessagesFor(string name)
		{
			return PullMessagesFor(name, x => true);
		}

		public bool WaitForNewMessages(string name)
		{
			return GetQueue(name).Events.Dequeue() != null;
		}

		public bool Exists(string name)
		{
			return queuesByName.ContainsKey(name);
		}

		public string WaitForNewMessages()
		{
			return messagesEvents.Dequeue();
		}

		public void Add(string name, TransportMessage message)
		{
			QueuePackage queue = GetQueue(name);
			lock(queue)
			{
				queue.Messages.AddFirst(message);
				queue.Events.Enqueue(new object());
				messagesEvents.Enqueue(name);
			}
		}

		private QueuePackage GetQueue(string name)
		{
			QueuePackage queue;
			if(queuesByName.TryGetValue(name, out queue)==false)
				throw new ArgumentException("Queue '" + name+ "' was not registered");
			return queue;
		}

		public class QueuePackage
		{
			public LinkedList<TransportMessage> Messages = new LinkedList<TransportMessage>();
			public IBlockingQueue<object> Events = new BlockingQueue<object>();
		}

		public void Dispose()
		{
			messagesEvents.Dispose();
		}
	}
}