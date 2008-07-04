using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BerkeleyDb
{
	public class BerkeleyDbEnvironment : DisposableMixin
	{
		private static readonly IDictionary<string, EnvContainer> environmentsByFullPath =
			new Dictionary<string, EnvContainer>(StringComparer.InvariantCultureIgnoreCase);
		private static readonly ReaderWriterLock locker = new ReaderWriterLock();

		private static Env AquireEnv(string fullPath)
		{
			locker.AcquireReaderLock(-1);
			try
			{
				EnvContainer container;
				if(environmentsByFullPath.TryGetValue(fullPath, out container))
					return container.AddRef();
				var cookie = locker.UpgradeToWriterLock(-1);
				try
				{
					if (environmentsByFullPath.TryGetValue(fullPath, out container))
						return container.AddRef();
					var env = new Env(EnvCreateFlags.None)
					{
						MaxLocks = 10000,
						MaxLockers = 10000,
						MaxLockObjects = 10000
					};
					env.Open(fullPath, CreationFlags, 0);
					environmentsByFullPath[fullPath] = container = new EnvContainer(env);
					return container.AddRef();
				}
				finally
				{
					locker.DowngradeFromWriterLock(ref cookie);
				}
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
		}

		private static void ReleaseEnv(string fullPath)
		{
			locker.AcquireReaderLock(-1);
			try
			{
				EnvContainer container;
				if (environmentsByFullPath.TryGetValue(fullPath, out container))
					return;
				lock (container)// only one thread can release 
				{
					if (container.Release() == false)
						return;
					var cookie = locker.UpgradeToWriterLock(-1);
					try
					{
						environmentsByFullPath.Remove(fullPath);
					}
					finally
					{
						locker.DowngradeFromWriterLock(ref cookie);
					}
				}
			}
			finally
			{
				locker.ReleaseReaderLock();
			}
		}

		private readonly Env inner;
		private BerkeleyDbTransaction currentTransaction;
		private readonly string fullEnvrionmentPath;

		public Env Inner
		{
			get { return inner; }
		}

		public BerkeleyDbTransaction CurrentTransaction
		{
			get { return currentTransaction; }
		}

		public BerkeleyDbEnvironment(string path)
		{
			// this protect us from failures if the current directory change
			fullEnvrionmentPath = Path.GetFullPath(path);
			inner = AquireEnv(fullEnvrionmentPath);
		}

		public bool IsTransactional
		{
			get { return (inner.EnvOpenFlags & Env.OpenFlags.InitTxn) != 0; }
		}

		public static Env.OpenFlags CreationFlags
		{
			get
			{
				return Env.OpenFlags.Create |
					   Env.OpenFlags.InitLock |
					   Env.OpenFlags.InitLog |
					   Env.OpenFlags.ThreadSafe |
					   Env.OpenFlags.Register |
					   Env.OpenFlags.InitMPool |
					   Env.OpenFlags.InitTxn |
					   Env.OpenFlags.Recover;
			}
		}

		protected override void Dispose(bool disposing)
		{
			// will dispose all current transactions, regardless of nesting
			while (currentTransaction != null)
				currentTransaction.Dispose();
			ReleaseEnv(fullEnvrionmentPath);
		}

		public BerkeleyDbTransaction BeginTransaction()
		{
			if (currentTransaction == null)
			{
				currentTransaction = new BerkeleyDbTransaction(this, 
					inner.TxnBegin(null, Txn.BeginFlags.ReadCommitted));
			}
			else
			{
				currentTransaction = currentTransaction.NestTransaction(
					tx => inner.TxnBegin(tx, Txn.BeginFlags.ReadCommitted));
			}
			return currentTransaction;
		}

		public void CreateQueue(string queueName, int queueItemSize)
		{
			var database = inner.CreateDatabase(DbCreateFlags.None);
			try
			{
				database.RecLen = queueItemSize;
				database.Open(CurrentTransaction.InnerTransaction(), queueName, null,
					DbType.Queue,
					Db.OpenFlags.Create, 0);
			}
			finally
			{
				OnTransactionDisposed(database.Close);
			}
		}

		private void OnTransactionDisposed(Action action)
		{
			if (currentTransaction == null)
			{
				action();
				return;
			}
			currentTransaction.RegisterSyncronization(action);
		}

		public bool DoesQueueExists(string queueName)
		{
			return File.Exists(Path.Combine(fullEnvrionmentPath, queueName));
		}

		internal void TransactionDisposed(BerkeleyDbTransaction transaction, BerkeleyDbTransaction parent)
		{
			if (transaction == currentTransaction)
				currentTransaction = parent;
		}

		public BerkeleyDbQueue OpenQueue(string queueName)
		{
			try
			{
				var database = inner.CreateDatabase(DbCreateFlags.None);
				var queue = (DbQueue)database.Open(CurrentTransaction.InnerTransaction(),
					queueName, null, DbType.Queue, Db.OpenFlags.None, 0);
				return new BerkeleyDbQueue(currentTransaction, database, queue);
			}
			catch (BdbException e)
			{
				if (e.Error == DbRetVal.ENOENT)
					throw new QueueDoesNotExistsException("Queue " + queueName + " does not exists", e);
				throw;
			}
		}

		public void DeleteQueue(string queueName)
		{
			try
			{
				inner.DbRemove(CurrentTransaction.InnerTransaction(),
				               queueName, null, Env.WriteFlags.None);
			}
			catch(BdbException e)
			{
				if (e.Error == DbRetVal.ENOENT)
					return;
				throw;
			}
		}
	}
}