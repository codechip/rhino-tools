using System;
using System.Collections.Generic;
using Rhino.Queues.Network;
using Rhino.Queues.Storage;

namespace Rhino.Queues.Impl
{
	public class QueueFactoryImpl : IQueueFactory
	{
		private readonly string name;
		private readonly IMessageStorage outgoingStorage, incomingStorage;
		private readonly IDictionary<string, string> destinationMapping;
		private Listener listener;
		private Sender sender;
		private bool started = false;

		public QueueFactoryImpl(string name, 
			IStorageFactory storageFactory, 
			IDictionary<string, string> destinationMapping,
			IEnumerable<string> queues, int listenerThreadCount, int senderThreadCount)
		{
			this.name = name;
			this.destinationMapping = destinationMapping;
			outgoingStorage = storageFactory.ForOutgoingMessages(new HashSet<string>(destinationMapping.Values));
			incomingStorage = storageFactory.ForIncomingMessages(new HashSet<string>(queues));
			listener = new Listener(this, listenerThreadCount, destinationMapping[name]);
			sender = new Sender(outgoingStorage, senderThreadCount);
		}

		public void Dispose()
		{
			incomingStorage.Dispose();
			outgoingStorage.Dispose();
			sender.Dispose();
			listener.Dispose();
			started = false;
		}

		public void Send(string destination, object msg)
		{
			AssertStarted();
			
			var dest = new Destination(this, destination);
			string endpointMapping;
			if (destinationMapping.TryGetValue(dest.Server, out endpointMapping) == false)
			{
				var message = string.Format(
					"Destination '{0}' endpoint was not registered. Did you forget to call Map('{0}').To('http://some/end/point');",
					dest.Server);
				throw new ArgumentException(message);
			}
			if (msg == null)
				throw new ArgumentNullException("msg");
			if(msg.GetType().IsSerializable==false)
				throw new ArgumentException("Message " + msg.GetType().Name + " must be serializable");

			outgoingStorage.Add(endpointMapping, new TransportMessage
			{
				Destination = dest,
				Message = msg,
				SentAt = SystemTime.Now()
			});
		}

		public IMessageQueue Queue(string queueName)
		{
			AssertStarted();
			return new MessageQueueImpl(queueName, incomingStorage);
		}

		private void AssertStarted()
		{
			if(started)
				return;
			throw new InvalidOperationException("Cannot send or queue before factory '" + name + "' is started");
		}

		public string Name
		{
			get { return name; }
		}

		public void Start()
		{
			sender.Start();
			listener.Start();
			started = true;
		}

		public bool HasQueue(string queueName)
		{
			return incomingStorage.Exists(queueName);
		}
	}
}