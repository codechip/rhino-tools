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
		private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

		private static Env AquireEnv(string fullPath)
		{
			locker.EnterUpgradeableReadLock();
			try
			{
				EnvContainer container;
				if (environmentsByFullPath.TryGetValue(fullPath, out container))
					return container.AddRef();
				locker.EnterWriteLock();
				try
				{
					if (environmentsByFullPath.TryGetValue(fullPath, out container))
						return container.AddRef();
					var env = new Env(EnvCreateFlags.None)
					{
						ErrorCall = delegate(Env env1, string errpfx, string msg) { Console.WriteLine(errpfx + ": " + msg); },
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
					locker.ExitWriteLock();
				}
			}
			finally
			{
				locker.ExitUpgradeableReadLock();
			}
		}

		private static void ReleaseEnv(string fullPath)
		{
			locker.EnterUpgradeableReadLock();
			try
			{
			    EnvContainer container;
			    if (environmentsByFullPath.TryGetValue(fullPath, out container) == false)
			        return;
			    if (container.Release() == false)
			        return;
			    locker.EnterWriteLock();
			    try
			    {
			        if (container.CanDispose == false)
			            return ;
			        environmentsByFullPath.Remove(fullPath);
			        container.Dispose();
			    }
			    finally
			    {
			        locker.ExitWriteLock();
			    }
			}
			finally
			{
			    locker.ExitUpgradeableReadLock();
			}
		}

		private readonly Env inner;
		private readonly string fullEnvrionmentPath;

		public Env Inner
		{
			get { return inner; }
		}

		public BerkeleyDbTransaction CurrentTransaction { get; private set; }

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
			while (CurrentTransaction != null)
				CurrentTransaction.Dispose();
			ReleaseEnv(fullEnvrionmentPath);
		}

		public BerkeleyDbTransaction BeginTransaction()
		{
			if (CurrentTransaction == null)
			{
				CurrentTransaction = new BerkeleyDbTransaction(this,
					inner.TxnBegin(null, Txn.BeginFlags.ReadCommitted));
			}
			else
			{
				CurrentTransaction = CurrentTransaction.NestTransaction(
					tx => inner.TxnBegin(tx, Txn.BeginFlags.ReadCommitted));
			}
			return CurrentTransaction;
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
			if (CurrentTransaction == null)
			{
				action();
				return;
			}
			CurrentTransaction.RegisterDisposeSyncronization(action);
		}

		public bool Exists(string queueName)
		{
			return File.Exists(Path.Combine(fullEnvrionmentPath, queueName));
		}

		internal void TransactionDisposed(BerkeleyDbTransaction transaction, BerkeleyDbTransaction parent)
		{
			if (transaction == CurrentTransaction)
				CurrentTransaction = parent;
		}

		public BerkeleyDbQueue OpenQueue(string queueName)
		{
			Db database = null;
			try
			{
				database = inner.CreateDatabase(DbCreateFlags.None);
				var queue = (DbQueue)database.Open(CurrentTransaction.InnerTransaction(),
												   queueName, null, DbType.Queue, Db.OpenFlags.ThreadSafe, 0);
				return new BerkeleyDbQueue(CurrentTransaction, database, queue);
			}
			catch (BdbException e)
			{
				if (database != null)
					OnTransactionDisposed(database.Close);
				if (e.Error == DbRetVal.ENOENT)
					throw new QueueDoesNotExistsException("Queue " + queueName + " does not exists", e);
				throw;
			}
		}

		public void Delete(string queueName)
		{
			try
			{
				inner.DbRemove(CurrentTransaction.InnerTransaction(),
							   queueName, null, Env.WriteFlags.None);
			}
			catch (BdbException e)
			{
				if (e.Error == DbRetVal.ENOENT)
					return;
				throw;
			}
		}

		public void CreateTree(string treeName)
		{
			var database = inner.CreateDatabase(DbCreateFlags.None);
			try
			{
				database.Open(CurrentTransaction.InnerTransaction(), treeName, null,
					DbType.BTree,
					Db.OpenFlags.Create, 0);
			}
			finally
			{
				OnTransactionDisposed(database.Close);
			}
		}

		public BerkeleyDbTree OpenTree(string treeName)
		{
			Db database = null;
			try
			{
				database = inner.CreateDatabase(DbCreateFlags.None);
				var tree = (DbBTree)database.Open(CurrentTransaction.InnerTransaction(),
												   treeName, null, DbType.BTree, Db.OpenFlags.ThreadSafe, 0);
				return new BerkeleyDbTree(CurrentTransaction, database, tree);
			}
			catch (BdbException e)
			{
				if (database != null)
					OnTransactionDisposed(database.Close);
				if (e.Error == DbRetVal.ENOENT)
					throw new QueueDoesNotExistsException("Tree " + treeName + " does not exists", e);
				throw;
			}
		}
	}
}