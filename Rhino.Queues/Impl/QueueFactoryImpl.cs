using System;
using System.Collections.Generic;
using Rhino.Queues.Network;
using Rhino.Queues.Storage;

namespace Rhino.Queues.Impl
{
	public class QueueFactoryImpl : IQueueFactoryImpl
	{
		private readonly string name;
		private readonly IMessageStorage outgoingStorage, incomingStorage;
		private readonly IDictionary<string, string> destinationMapping;
		private readonly IListener listener;
		private readonly ISender sender;
		private bool started;

		public void MarkAsStarted()
		{
			started = true;
		}

		public IMessageStorage OutgoingStorage
		{
			get { return outgoingStorage; }
		}

		public IMessageStorage IncomingStorage
		{
			get { return incomingStorage; }
		}

		public QueueFactoryImpl(string name, IStorageFactory storageFactory, IDictionary<string, string> destinationMapping, IEnumerable<string> queues, IListenerFactory listenerFactory, ISenderFactory senderFactory)
		{
			this.name = name;
			this.destinationMapping = destinationMapping;
			outgoingStorage = storageFactory.ForOutgoingMessages(new HashSet<string>(destinationMapping.Values));
			incomingStorage = storageFactory.ForIncomingMessages(new HashSet<string>(queues));
			listener = listenerFactory.Create(this, destinationMapping[name]);
			sender = senderFactory.Create(outgoingStorage);
		}

		public void Dispose()
		{
			incomingStorage.Dispose();
			outgoingStorage.Dispose();
			sender.Dispose();
			listener.Dispose();
			started = false;
		}

		public IMessageQueue OpenQueue(string queueName)
		{
			return OpenQueueImpl(queueName);
		}

		public IMessageQueueImpl OpenQueueImpl(string queueName)
		{
			AssertStarted();
			return new MessageQueueImpl(
				new Destination(queueName, this.Name),
				incomingStorage,
				outgoingStorage,
				this);
		}


		private void AssertStarted()
		{
			if (started)
				return;
			throw new InvalidOperationException("Cannot send or queue before factory '" + name + "' is started");
		}

		public string Name
		{
			get { return name; }
		}

		public void Start()
		{
			sender.Error += OnSendError;
			sender.Start();
			listener.Start();
			started = true;
		}

		public void OnSendError(Exception exception, TransportMessage msg, MessageSendFailure failure)
		{
			msg.FailureCount += 1;
			DeliveryFailure(msg, exception);
			if (msg.FailureCount > 500 || failure != MessageSendFailure.None)
			{
				FinalDeliveryFailure(msg, exception);
				return;
			}
			msg.SendAt = SystemTime.Now().AddSeconds(msg.FailureCount * 2);
			var destination = GetEndpointFromDestination(msg.Destination);
			outgoingStorage.Add(destination, msg);
		}
		public string GetEndpointFromDestination(Destination destination)
		{
			string endpointMapping;
			if (destinationMapping.TryGetValue(destination.Server, out endpointMapping) == false)
			{
				var message = string.Format(
					"Destination '{0}' endpoint was not registered. Did you forget to call Map('{0}').To('http://some/end/point');",
					destination.Server);
				throw new ArgumentException(message);
			}
			return endpointMapping;
		}

		public bool HasQueue(string queueName)
		{
			return incomingStorage.Exists(queueName);
		}

		public event Action<TransportMessage, Exception> DeliveryFailure = delegate { };
		public event Action<TransportMessage, Exception> FinalDeliveryFailure = delegate { };
	}
}