using System;

namespace Rhino.Queues
{
	public interface ILocalQueue : IRemoteQueue
	{
		event Action<QueueMessage> MessageArrived;

		/// <summary>
		/// Recieves a queue message from the queue, with 
		/// a timeout of 1 minute
		/// </summary>
		/// <returns></returns>
		QueueMessage Recieve();
		QueueMessage Recieve(TimeSpan timeout);
	}
}