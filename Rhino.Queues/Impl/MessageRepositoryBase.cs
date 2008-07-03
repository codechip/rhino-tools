using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Rhino.Queues.Impl
{
	public abstract class MessageRepositoryBase
	{
		private readonly Func<IDbConnection> createConnection;
		private readonly BinaryFormatter formatter = new BinaryFormatter();

		protected MessageRepositoryBase(string name, string queuesDirectory)
		{
			databaseFilename = Path.Combine(queuesDirectory, name) + ".queue";
			createConnection = delegate
			{
				var con = new SQLiteConnection("Data Source=" + this.databaseFilename);
				for (int i = 0; i < 10; i++)
				{
					try
					{
						con.Open();
						break;
					}
					catch (SQLiteException e)
					{
						if (e.ErrorCode == SQLiteErrorCode.Busy)
							continue;
						throw;
					}
				}
				// if still failed after 10 tries, we need to let the exception bubble up
				if (con.State != ConnectionState.Open)
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
				using (var connection = new SQLiteConnection("Data Source=" + databaseFilename + ";New=true"))
				using (var command = connection.CreateCommand())
				{
					connection.Open();
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
			AddParameter(cmd, name, value);
		}

		protected static void AddParameter(IDbCommand command, string name, object value)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			command.Parameters.Add(parameter);
		}


		[ThreadStatic]
		protected static IDbCommand cmd;
		[ThreadStatic]
		protected static IDbConnection connection;
		[ThreadStatic]
		protected static IDbTransaction transaction;

		private string databaseFilename;

		public void Transaction(Action action)
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
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
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