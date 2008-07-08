using Rhino.Queues.Impl;

namespace Rhino.Queues.Network
{
	public class ListenerFactory : IListenerFactory
	{
		private int workerThreadsCount;

		public ListenerFactory(int workerThreadsCount)
		{
			this.workerThreadsCount = workerThreadsCount;
		}

		public void Dispose()
		{
			
		}

		public IListener Create(IQueueFactoryImpl queueFactory,  string endpoint)
		{
			return new Listener(queueFactory, workerThreadsCount, endpoint);
		}
	}
}