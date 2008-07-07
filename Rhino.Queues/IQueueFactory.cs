using System;

namespace Rhino.Queues
{
	public interface IQueueFactory : IDisposable
	{
		void Send(string destination, object msg);
		IMessageQueue Queue(string queueName);
		string Name { get; }
		void Start();
		bool HasQueue(string queueName);
	}
}