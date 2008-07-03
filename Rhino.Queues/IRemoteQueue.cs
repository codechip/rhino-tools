using System;

namespace Rhino.Queues
{
	public interface IRemoteQueue
	{
		Uri Url { get; }
		void Send(QueueMessage message);
	}
}