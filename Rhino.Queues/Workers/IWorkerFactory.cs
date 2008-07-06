using Rhino.Queues.Data;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Workers
{
	public interface IWorkerFactory
	{
		void StartWorkers(
			IQueueFactoryImpl factory, 
			IOutgoingMessageRepository outgoingMessageRepository
			);

		void StopWorkers();

		void WaitForAllWorkersToStop();

		void NotifyAllWorkersThatNewMessageWasStored();
	}
}