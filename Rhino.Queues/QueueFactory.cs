using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rhino.Queues.Extensions;
using Rhino.Queues.Impl;
using Rhino.Queues.Workers;

namespace Rhino.Queues
{
	/// <summary>
	/// There should be only a single queue factory in the application
	/// </summary>
	public class QueueFactory : IQueueFactoryImpl
	{
		private readonly Uri localUri;
		private readonly ReaderWriterLock locker = new ReaderWriterLock();
		private readonly IOutgoingMessageRepository outgoingMessageRepository;
		private readonly IQueueListener queueListener;

		private readonly Dictionary<Uri, ILocalQueue> queuesByDestinationUrl =
			new Dictionary<Uri, ILocalQueue>();

		private readonly Dictionary<string, ILocalQueue> queuesByName =
			new Dictionary<string, ILocalQueue>(StringComparer.InvariantCultureIgnoreCase);

		private readonly string queuesDirectory;
		private readonly IQueuePhysicalStorage queuePhysicalStorage;
		private readonly IWorkerFactory workerFactory;
		private bool initialized;

		public QueueFactory(
			Uri localUri,
			string queuesDirectory,
			IQueuePhysicalStorage queuePhysicalStorage,
			IWorkerFactory workerFactory,
			IQueueListener queueListener,
			IOutgoingMessageRepository outgoingMessageRepository)
		{
			ValidationUtil.ValidateQueueUrl(localUri);
			this.localUri = localUri;
			this.queuesDirectory = queuesDirectory;
			this.queuePhysicalStorage = queuePhysicalStorage;
			this.workerFactory = workerFactory;
			this.queueListener = queueListener;
			this.outgoingMessageRepository = outgoingMessageRepository;
		}

		#region IQueueFactoryImpl Members

		public Uri LocalUri
		{
			get { return localUri; }
		}

		public IQueueImpl[] GetAllLocalQueues()
		{
			locker.AcquireReaderLock(-1);
			try
			{
				return queuesByName.Values.Cast<IQueueImpl>().ToArray();
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
		}

		public void Initialize()
		{
			if (initialized)
				throw new InvalidOperationException("The queue factory was already initialized");
			initialized = true;

			CreateQueuesFromStorage();

			// we start without any batches active, so let us clean up
			// any that were in the middle of a batch when we last finished
			outgoingMessageRepository.ResetAllBatches();
			workerFactory.StartWorkers(this, outgoingMessageRepository);
			outgoingMessageRepository.NewMessageStored += () => workerFactory.NotifyAllWorkersThatNewMessageWasStored();
			queueListener.Start(this);
		}

		public void CreateQueuesFromStorage()
		{
			foreach (var queueName in queuePhysicalStorage.GetQueueNames())
			{
				CreateLocalQueueInstance(queueName);
			}
		}

		public void CreateQueue(string queueName)
		{
			queuePhysicalStorage.CreateInputQueue(queueName);
			CreateLocalQueueInstance(queueName);
		}

		public IRemoteQueue GetRemoteQueue(string queueUrl)
		{
			return GetRemoteQueue(new Uri(queueUrl));
		}

		public IRemoteQueue GetRemoteQueue(Uri queueUrl)
		{
			ValidationUtil.ValidateQueueUrl(queueUrl);
			var localQueueName = queueUrl.ToQueueName();
			AssertQueueNameIsNotOutgoingQueue(localQueueName);
			if (IsLocal(queueUrl))
				return GetLocalQueue(localQueueName);
			locker.AcquireReaderLock(-1);
			try
			{
				ILocalQueue queue;
				if (queuesByDestinationUrl.TryGetValue(queueUrl, out queue) == false)
				{
					var writerLock = locker.UpgradeToWriterLock(-1);
					try
					{
						if (queuesByDestinationUrl.TryGetValue(queueUrl, out queue) == false)
						{
							var repository = new IncomingMessageRepository(localQueueName, queuesDirectory);
							queuesByDestinationUrl[queueUrl] = queue = new Queue(
								queueUrl,
								outgoingMessageRepository,
								repository);
						}
					}
					finally
					{
						locker.DowngradeFromWriterLock(ref writerLock);
					}
				}
				return queue;
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
		}

		public ILocalQueue GetLocalQueue(string queueName)
		{
			AssertQueueNameIsNotOutgoingQueue(queueName);
			ILocalQueue queue;
			locker.AcquireReaderLock(-1);
			try
			{
				queuesByName.TryGetValue(queueName, out queue);
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
			return queue;
		}

		private void CreateLocalQueueInstance(string queueName)
		{
			locker.AcquireReaderLock(-1);
			try
			{
				ILocalQueue queue;
				if (queuesByName.TryGetValue(queueName, out queue) == false)
				{
					var writerLock = locker.UpgradeToWriterLock(-1);
					try
					{
						if (queuesByName.TryGetValue(queueName, out queue) == false)
						{
							var repository = new IncomingMessageRepository(queueName, queuesDirectory);
							queuesByName[queueName] = new Queue(
							                                  	new UriBuilder("queue", "localhost", -1, queueName).Uri,
							                                  	outgoingMessageRepository,
							                                  	repository);
						}
					}
					finally
					{
						locker.DowngradeFromWriterLock(ref writerLock);
					}
				}
				return;
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
		}

		private void AssertQueueNameIsNotOutgoingQueue(string localQueueName)
		{
			if (localQueueName.Equals(outgoingMessageRepository.Name, StringComparison.InvariantCultureIgnoreCase))
				throw new InvalidOperationException("The outgoing queue '" + localQueueName + "' is reserved for Rhino Queues use");
		}

		public bool IsLocal(Uri uri)
		{
			if (uri.IsLoopback == false)
				return false;
			int uriPort = uri.Port == -1 ? 80 : uri.Port;
			int localPort = localUri.Port == -1 ? 80 : localUri.Port;
			if (uriPort != localPort)
				return false;
			return uri.LocalPath.Equals(localUri.LocalPath, StringComparison.InvariantCultureIgnoreCase);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		~QueueFactory()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (queueListener!=null) 
				queueListener.Stop();

			if (workerFactory != null) 
				workerFactory.StopWorkers();

			if (workerFactory != null) 
				workerFactory.WaitForAllWorkersToStop();
		}
	}
}