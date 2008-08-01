using System;
using Rhino.Queues.Storage;
using System.Linq;

namespace Rhino.Queues.Impl
{
	public class DiskMessageQueueImpl : IMessageQueueImpl
	{
		private readonly Destination destination;
		private readonly IMessageStorage incomingStorage;
		private readonly IMessageStorage outgoingStorage;
		private readonly IQueueFactoryImpl queueFactory;

		public DiskMessageQueueImpl(
			Destination destination,
			IMessageStorage incomingStorage,
			IMessageStorage outgoingStorage,
			IQueueFactoryImpl queueFactory)
		{
			this.destination = destination;
			this.incomingStorage = incomingStorage;
			this.outgoingStorage = outgoingStorage;
			this.queueFactory = queueFactory;
		}

		public Message Send(object msg)
		{
			string endpointMapping = queueFactory.GetEndpointFromDestination(destination);
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (msg.GetType().IsSerializable == false)
				throw new ArgumentException("Message " + msg.GetType().Name + " must be serializable");

			var now = SystemTime.Now();
			var message = new Message
			{
				Value = msg,
				Destination = destination,
				SentAt = now,
			};
			outgoingStorage.Add(endpointMapping, new TransportMessage
			{
				Destination = destination,
				Message = message,
				SendAt = now
			});

			return message;
		}

		public Message Recieve()
		{
			var message = incomingStorage.PullMessagesFor(destination.Queue).FirstOrDefault();
			while (message == null)
			{
				if (incomingStorage.WaitForNewMessages(destination.Queue) == false)
					return null;
				message = incomingStorage.PullMessagesFor(destination.Queue).FirstOrDefault();
			}
			incomingStorage.Add(destination.Queue, message);
			return message.Message;
		}


		public void PutAll(TransportMessage[] msgs)
		{
			foreach (var msg in msgs)
			{
				incomingStorage.Add(destination.Queue, msg);
			}
		}

		public void Dispose()
		{

		}
	}
}