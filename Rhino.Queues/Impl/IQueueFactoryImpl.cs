using System;

namespace Rhino.Queues.Impl
{
	public interface IQueueFactoryImpl : IQueueFactory
	{
		bool IsLocal(Uri uri);
		Uri LocalUri { get; }

		IQueueImpl[] GetAllLocalQueues();
	}
}