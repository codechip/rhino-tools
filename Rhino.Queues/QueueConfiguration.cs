using System;
using System.IO;
using Rhino.Queues.Impl;
using Rhino.Queues.Workers;
using System.Linq;

namespace Rhino.Queues
{
	public class QueueConfiguration
	{
		private int workersCount = 3;
		private bool skipInitialization;
		private Uri localUri = new Uri("queue://localhost/rhino-queues");
		private string queuesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Queues");
		private bool purgePendingMessages;

		public QueueConfiguration WorkerThreads(int count)
		{
			workersCount = count;
			return this;
		}

		public QueueConfiguration LocalUri(string uri)
		{
			return LocalUri(new Uri(uri));
		}

		public QueueConfiguration LocalUri(Uri uri)
		{
			ValidationUtil.ValidateQueueUrl(uri);
			localUri = uri;
			return this;
		}

		public QueueConfiguration QueuesDirectory(string path)
		{
			queuesDirectory = path;
			return this;
		}

		public QueueConfiguration SkipInitializingTheQueueFactory()
		{
			skipInitialization = true;
			return this;
		}

		public IQueueFactory BuildQueueFactory()
		{
			if (Directory.Exists(queuesDirectory) == false)
				Directory.CreateDirectory(queuesDirectory);

			var queuePhysicalStorage = new IncomingQueuePhysicalStorage(queuesDirectory);
			var outgoingQueueName = "outgoing-msgs";

			var repository = new OutgoingMessageRepository(outgoingQueueName, queuesDirectory);
			if (queuePhysicalStorage.GetQueueNames().Contains(outgoingQueueName) == false)
			{
				repository.CreateQueueStorage();
			}

			var factory = new QueueFactory(
				localUri,
				queuesDirectory,
				queuePhysicalStorage,
				new WorkerFactory(workersCount),
				new QueueListener(localUri),
				repository);

			factory.CreateQueuesFromStorage();
			if (purgePendingMessages)
			{
				foreach (var queue in factory.GetAllLocalQueues())
				{
					queue.PurgeAllMessages();
				}
			}
			if (skipInitialization == false)
				factory.Initialize();

			return factory;
		}

		public QueueConfiguration PurgePendingMessages()
		{
			purgePendingMessages = true;
			return this;
		}
	}
}