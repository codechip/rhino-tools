using System.Collections.Generic;
using System.Threading;
using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Workers
{
	public class WorkerFactory : IWorkerFactory
	{
		private readonly int workersCount;
		private readonly IList<IQueueWorker> workers = new List<IQueueWorker>();
		private readonly IList<Thread> threads = new List<Thread>();

		public IList<IQueueWorker> Workers
		{
			get { return workers; }
		}

		public IList<Thread> Threads
		{
			get { return threads; }
		}

		public WorkerFactory(int workersCount)
		{
			this.workersCount = workersCount;
		}

		public void StartWorkers(IQueueFactoryImpl factory, IOutgoingMessageRepository outgoingMessageRepository)
		{
			for (int i = 0; i < workersCount; i++)
			{
				var worker = new QueueWorker(
					new CommandFactory(factory),
					factory, 
					outgoingMessageRepository);
				workers.Add(worker);
				threads.Add(new Thread(worker.Run)
				{
					IsBackground = true,
					Name = "Queue Worker #" + i
				});
			}
			foreach (var thread in threads)
			{
				thread.Start();
			}
		}

		public void StopWorkers()
		{
			foreach (var worker in workers)
			{
				worker.Stop();
			}
		}

		public void WaitForAllWorkersToStop()
		{
			foreach (var thread in threads)
			{
				thread.Join();
			}
		}

		public void NotifyAllWorkersThatNewMessageWasStored()
		{
			foreach (var worker in workers)
			{
				worker.NewMessageStored();
			}
		}
	}
}