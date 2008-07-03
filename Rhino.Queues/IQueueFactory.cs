using System;

namespace Rhino.Queues
{
	public interface IQueueFactory : IDisposable
	{
		void Initialize();

		void CreateQueue(string queueName);

		ILocalQueue GetLocalQueue(string queueName);
		IRemoteQueue GetRemoteQueue(Uri queueUri);
		IRemoteQueue GetRemoteQueue(string queueUri);
	}
}