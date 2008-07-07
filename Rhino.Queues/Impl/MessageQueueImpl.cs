using Rhino.Queues.Storage;
using System.Linq;

namespace Rhino.Queues.Impl
{
	public class MessageQueueImpl : IMessageQueue
	{
		private readonly string queueName;
		private readonly IMessageStorage storage;

		public MessageQueueImpl(string queueName, IMessageStorage storage)
		{
			this.queueName = queueName;
			this.storage = storage;
		}

		public object Recieve()
		{
			var message = storage.GetMessagesFor(queueName).FirstOrDefault();
			while(message==null)
			{
				if(storage.WaitForNewMessages(queueName) == false)
					return null;
				message = storage.GetMessagesFor(queueName).FirstOrDefault();
			}
			return message.Message;
		}

		public void PutAll(TransportMessage[] msgs)
		{
			foreach (var msg in msgs)
			{
				storage.Add(queueName, msg);
			}
		}
	}
}