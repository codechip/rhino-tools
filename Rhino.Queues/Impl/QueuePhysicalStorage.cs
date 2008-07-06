using System.Collections.Generic;
using System.IO;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Data;

namespace Rhino.Queues.Impl
{
	public class QueuePhysicalStorage : IQueuePhysicalStorage
	{
		private readonly string path;
		private const string trackingDatabaseName = "rhino-queues-queues-list";

		public QueuePhysicalStorage(string path)
		{
			this.path = path;
		}

		private void EnsureQueuesTrackingDatabaseExists()
		{
			if (File.Exists(Path.Combine(path, trackingDatabaseName) + ".fdb"))
				return;

			lock (this)
			{
				if (File.Exists(Path.Combine(path, trackingDatabaseName) + ".fdb"))
					return;

				var connectionString = GetConnectionString(path, trackingDatabaseName);
				FbConnection.CreateDatabase(connectionString);
				using (var connection = new FbConnection(connectionString))
				using (var cmd = connection.CreateCommand())
				{
					connection.Open();
					var commands = Queries.CreateTablesForQueuesList.Split(';');
					foreach (var command in commands)
					{
						if (command.Trim().Length == 0)
							continue;
						cmd.CommandText = command;
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		public void CreateInputQueue(string queueName)
		{
			CreateDatabase(queueName, Queries.CreateTablesForIncomingQueue, QueueType.Input);
		}

		private void CreateDatabase(string queueName, string ddl, QueueType type)
		{
			EnsureQueuesTrackingDatabaseExists();
			var connectionString = GetConnectionString(path, queueName);
			FbConnection.CreateDatabase(connectionString);
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				var commands = ddl.Split(';');
				foreach (var command in commands)
				{
					if (command.Trim().Length == 0)
						continue;
					cmd.CommandText = command;
					cmd.ExecuteNonQuery();
				}
			}

			connectionString = GetConnectionString(path, trackingDatabaseName);
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				using(var tx = connection.BeginTransaction())
				{
					cmd.Transaction = tx;
					cmd.CommandText = Queries.InsertNewQuque;
					cmd.Parameters.Add("@Name", queueName);
					cmd.Parameters.Add("@Type", type);
					cmd.ExecuteNonQuery();
					tx.Commit();
				}
			}
		}

		public string[] GetIncomingQueueNames()
		{
			return GetQueues(QueueType.Input);
		}

		private string[] GetQueues(QueueType queueType)
		{
			EnsureQueuesTrackingDatabaseExists();
			var queues = new List<string>();
			var connectionString = GetConnectionString(path, trackingDatabaseName);
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				using (var tx = connection.BeginTransaction())
				{
					cmd.Transaction = tx;
					cmd.CommandText = Queries.SelectQueues;
					cmd.Parameters.Add("@Type", queueType);
					using (var reader = cmd.ExecuteReader())
					{
						while(reader.Read())
						{
							queues.Add(reader.GetString(0));
						}
					}
					tx.Commit();
				}
			}
			return queues.ToArray();
		}

		public void CreateOutputQueue(string queueName)
		{
			CreateDatabase(queueName, Queries.CreateTablesForOutgoingQueue, QueueType.Output);
		}

		public static string GetConnectionString(string path, string name)
		{
			return string.Format("Database={0}.fdb;ServerType=1",
								 Path.Combine(Path.GetFullPath(path), name));
		}

		public string[] GetOutgoingQueueNames()
		{
			return GetQueues(QueueType.Output);
		}
	}
}