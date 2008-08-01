using System;
using System.Collections.Generic;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Storage.Disk
{
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using Threading;

	public class DiskMessageStorage : MessageStorageBase
	{
		[ThreadStatic]
		private static IDictionary<string, IPersistentQueueSession> queueSessionsByName;

		private readonly IDictionary<string, IPersistentQueue> queuesByName =
			new Dictionary<string, IPersistentQueue>(StringComparer.InvariantCultureIgnoreCase);

		private readonly IDictionary<string, IBlockingQueue<object>> queuesEventsByName =
			new Dictionary<string, IBlockingQueue<object>>(StringComparer.InvariantCultureIgnoreCase);

		public DiskMessageStorage(string basePath, IEnumerable<KeyValuePair<string, string>> queues)
		{
			foreach (var endpoint in queues)
			{
				queuesByName.Add(endpoint.Key, new PersistentQueue(Path.Combine(basePath, endpoint.Value)));
				queuesEventsByName.Add(endpoint.Key, new BlockingQueue<object>());
			}
		}
		public override IEnumerable<TransportMessage> PullMessagesFor(string name, Predicate<TransportMessage> predicate)
		{
			var session = GetQueueSession(name);
			var rejectedEntires = new HashSet<byte[]>();
			while (true)
			{
				Action reverse;
				var entry = session.ReversibleDequeue(out reverse);
				if (entry == null)
					yield break;
				// we have gone a full cycle, and are now reading the 
				// items we rejected previous, break to avoid infinite loop
				if (rejectedEntires.Contains(entry))
					yield break;
				var transportMessage = Deserialize(entry);
				if (predicate(transportMessage) == false)
				{
					rejectedEntires.Add(entry);
					reverse();
					continue;
				}
				yield return transportMessage;
			}
		}

		private IPersistentQueueSession GetQueueSession(string name)
		{
			if (queueSessionsByName == null)
				queueSessionsByName = new Dictionary<string, IPersistentQueueSession>(StringComparer.InvariantCultureIgnoreCase);
			IPersistentQueueSession session;
			if (queueSessionsByName.TryGetValue(name, out session) == false)
				queueSessionsByName[name] = session = GetQueue(name).OpenSession();
			if (session.IsUsable)
				return session;
			queueSessionsByName[name] = session = GetQueue(name).OpenSession();
			return session;
		}

		private static TransportMessage Deserialize(byte[] entryData)
		{
			using (var stream = new MemoryStream(entryData))
			{
				return (TransportMessage)new BinaryFormatter().Deserialize(stream);
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
			var session = GetQueueSession(name);
			session.Enqueue(Serializable(message));
		}

		private static byte[] Serializable(TransportMessage message)
		{
			using (var ms = new MemoryStream())
			{
				new BinaryFormatter().Serialize(ms, message);
				return ms.ToArray();
			}
		}

		private IPersistentQueue GetQueue(string name)
		{
			IPersistentQueue queue;
			if (queuesByName.TryGetValue(name, out queue) == false)
				throw new ArgumentException("Queue '" + name + "' was not registered");
			return queue;
		}

		private IBlockingQueue<object> GetQueueEvents(string name)
		{
			IBlockingQueue<object> queue;
			if (queuesEventsByName.TryGetValue(name, out queue) == false)
				throw new ArgumentException("Queue '" + name + "' was not registered");
			return queue;
		}

		public override bool WaitForNewMessages(string name)
		{
			object ignored;
			return GetQueueEvents(name).Dequeue(TimeSpan.FromMilliseconds(Timeout.Infinite), out ignored);
		}
	}
}