using System;
using System.Collections.Generic;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Storage.InMemory
{
	public class InMemoryMessageStorage : MessageStorageBase
	{
		private readonly IDictionary<string, QueuePackage> queuesByName =
			new Dictionary<string, QueuePackage>(StringComparer.InvariantCultureIgnoreCase);


		public InMemoryMessageStorage(IEnumerable<string> queues)
		{
			foreach (var endpoint in new HashSet<string>(queues))
			{
				queuesByName.Add(endpoint, new QueuePackage());
			}
		}
		public override IEnumerable<TransportMessage> PullMessagesFor(string name, Predicate<TransportMessage> predicate)
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
		public override IEnumerable<TransportMessage> PullMessagesFor(string name)
		{
			return PullMessagesFor(name, x => true);
		}

		public override bool Exists(string name)
		{
			return queuesByName.ContainsKey(name);
		}

		public override IEnumerable<string> Queues
		{
			get
			{
				return queuesByName.Keys;
			}
		}

		protected override void OnAdd(string name, TransportMessage message)
		{
			QueuePackage queue = GetQueue(name);
			lock (queue)
			{
				queue.Add(message);
			}
		}

		private QueuePackage GetQueue(string name)
		{
			QueuePackage queue;
			if (queuesByName.TryGetValue(name, out queue) == false)
				throw new ArgumentException("Queue '" + name + "' was not registered");
			return queue;
		}

		public override bool WaitForNewMessages(string name)
		{
			return GetQueue(name).WaitForNewMessage();
		}
	}
}