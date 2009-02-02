namespace Rhino.ServiceBus.Msmq
{
	using System;
	using System.Messaging;

	public class QueueInfo
	{
		public string QueuePath { get; set; }
		public Uri QueueUri { get; set; }
		public bool IsLocal { get; set; }

		public bool Exists
		{
			get
			{
				if (IsLocal)
					return MessageQueue.Exists(QueuePath);
				return true; // we assume that remote queues exists
			}
		}

		public MessageQueue Open()
		{
			return new MessageQueue(QueuePath);
		}

		public MessageQueue Open(QueueAccessMode access)
		{
			return new MessageQueue(QueuePath, access);
		}

		public void Delete()
		{
			if(Exists && IsLocal)
				MessageQueue.Delete(QueuePath);
		}

		public void Create()
		{
			if (IsLocal==false || Exists)
				return;
			MessageQueue.Create(QueuePath, true);
		}
	}
}