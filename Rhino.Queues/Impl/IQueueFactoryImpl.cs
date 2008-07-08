using Rhino.Queues.Storage;

namespace Rhino.Queues.Impl
{
	public interface IQueueFactoryImpl : IQueueFactory
	{
		IMessageQueueImpl OpenQueueImpl(string queueName);
		string GetEndpointFromDestination(Destination destination);
	}
}