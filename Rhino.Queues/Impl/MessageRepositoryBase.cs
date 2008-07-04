using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using log4net;

namespace Rhino.Queues.Impl
{
	public abstract class MessageRepositoryBase
	{
		private readonly Func<IDbConnection> createConnection;
		private readonly BinaryFormatter formatter = new BinaryFormatter();
		private ILog logger;

		protected MessageRepositoryBase(string name, string queuesDirectory)
		{
			logger = LogManager.GetLogger(GetType());
			databaseFilename = Path.Combine(queuesDirectory, name) + ".queue";
			createConnection = delegate
			{
				var con = new SQLiteConnection("Data Source=" + databaseFilename);
				con.Open();
				return con;
			};
		}

		public Func<IDbConnection> CreateConnection
		{
			get { return createConnection; }
		}

		public void CreateQueueStorage()
		{
			if (File.Exists(databaseFilename) == false)
			{
				using (var con = new SQLiteConnection("Data Source=" + databaseFilename + ";New=true"))
				using (var command = con.CreateCommand())
				{
					con.Open();
					command.CommandText = GetQueryToGenerateDatabase();
					command.ExecuteNonQuery();
				}
			}
		}

		protected abstract string GetQueryToGenerateDatabase();

		protected byte[] Serialize(QueueMessage msg)
		{
			var stream = new MemoryStream();
			formatter.Serialize(stream, msg);
			return stream.ToArray();
		}

		protected QueueMessage Deserialize(byte[] data)
		{
			var stream = new MemoryStream(data);
			return (QueueMessage)formatter.Deserialize(stream);
		}

		protected static void AddParameter(string name, object value)
		{
			var parameter = cmd.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			cmd.Parameters.Add(parameter);
		}


		[ThreadStatic]
		protected static IDbCommand cmd;
		[ThreadStatic]
		protected static IDbConnection connection;
		[ThreadStatic]
		protected static IDbTransaction transaction;

		private readonly string databaseFilename;

		/// <summary>
		/// Execute the specified a transaction.
		/// Note that code the action is assumed to be transactional as well.
		/// That is, if the transaction have failed, it has no effect.
		/// This is important since we may execute the action several time,
		/// to overcome deadlock exceptions
		/// </summary>
		/// <param name="action">The action.</param>
		public void Transaction(Action action)
		{
			TransactionWithRetries(action, 5);
		}

		private void TransactionWithRetries(Action action, int retryCount)
		{
			if (cmd != null)
			{
				action();
				return;
			}
			try
			{
				using (connection = CreateConnection())
				using (transaction = connection.BeginTransaction(IsolationLevel.Serializable))
				using (cmd = connection.CreateCommand())
				{
					cmd.Transaction = transaction;
					action();
					transaction.Commit();
				}
			}
			catch (DbException e)
			{
				// since we have tests to validate that the SQL we execute is valid,
				// we assume that any DB error is transient and likely because of locking
				// so we will try it again after a short pause
				if (retryCount > 0)
				{
					logger.Warn("Error executing transaction, retrying", e);
					Thread.Sleep(200);
					TransactionWithRetries(action, retryCount - 1);
				}
				else
				{
					logger.Error("Failed to execute transaction, aborting transaction", e);
					throw;
				}
			}
			finally
			{
				cmd = null;
				transaction = null;
				connection = null;
			}
		}
	}
}