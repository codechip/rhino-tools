using Rhino.Queues.Storage;

namespace Rhino.Queues.Network
{
	public class SenderFactory : ISenderFactory
	{
		int workerThreadsCount;

		public SenderFactory(int workerThreadsCount)
		{
			this.workerThreadsCount = workerThreadsCount;
		}

		public void Dispose()
		{
			
		}

		public ISender Create(IMessageStorage storage)
		{
			return new Sender(storage, workerThreadsCount);
		}
	}
}