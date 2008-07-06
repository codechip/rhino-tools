using System;
using System.IO;
using Rhino.Queues.Data;
using Rhino.Queues.Extensions;
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
		private string name;

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

			var queuePhysicalStorage = new QueuePhysicalStorage(queuesDirectory);
			var outgoingQueueName = "outgoing-msgs";

			var repository = new OutgoingMessageRepository(outgoingQueueName, queuesDirectory);
			if (queuePhysicalStorage.GetOutgoingQueueNames().Contains(outgoingQueueName, StringComparer.InvariantCultureIgnoreCase) == false)
			{
				queuePhysicalStorage.CreateOutputQueue(outgoingQueueName);
			}

			name = name ?? Path.GetFileName(queuesDirectory);
			var factory = new QueueFactory(
				localUri,
				queuesDirectory,
				queuePhysicalStorage,
				new WorkerFactory(workersCount, name),
				new QueueListener(localUri),
				repository);

			if (queuePhysicalStorage.GetIncomingQueueNames().Contains(localUri.ToQueueName(), StringComparer.InvariantCultureIgnoreCase) == false)
			{
				queuePhysicalStorage.CreateInputQueue(localUri.ToQueueName());
			}

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

		public QueueConfiguration Name(string name)
		{
			this.name = name;
			return this;
		}

	}
}