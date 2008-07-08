using System;
using Rhino.Queues.Impl;

namespace Rhino.Queues
{
	public interface IQueueFactory : IDisposable
	{
		IMessageQueue OpenQueue(string queueName);
		string Name { get; }
		void Start();
		bool HasQueue(string queueName);
		event Action<TransportMessage, Exception> FinalDeliveryFailure;
	}
}