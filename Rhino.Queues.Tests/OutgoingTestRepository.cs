using System;
using System.Collections.Generic;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Data;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	public class OutgoingTestRepository : IDisposable
	{
		private readonly string connectionString;

		public OutgoingTestRepository(string name)
		{
			connectionString = QueuePhysicalStorage.GetConnectionString(name, name);
		}

		public QueueTransportMessage[] GetTransportMessages()
		{
			var msgs = new List<QueueTransportMessage>();
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open(); 
				cmd.CommandText = "SELECT Destination, Data, SendAt FROM OutgoingMessages";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						msgs.Add(new QueueTransportMessage
						{
							Destination = new Uri(reader.GetString(0)),
							Message = DataAccessMixin.Deserialize((byte[]) reader[1]),
							SendAt = reader.GetDateTime(2)
						});
					}
				}
				return msgs.ToArray();
			}
		}

		public int[] GetTransportMessagesFailures()
		{
			var msgs = new List<int>();
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT FailureCount FROM OutgoingMessages";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						msgs.Add(reader.GetInt32(0));
					}
				}
				return msgs.ToArray();
			}
		}


		public FailedQueueMessage[] GetDeadLetters()
		{
			var msgs = new List<FailedQueueMessage>();
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT Destination, Data, FinalFailureAt, LastException FROM FailedMessages";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						msgs.Add(new FailedQueueMessage
						{
							Destination = new Uri(reader.GetString(0)),
							Message = DataAccessMixin.Deserialize((byte[])reader[1]),
							FinalFailureAt = reader.GetDateTime(2),
                            Exception = new Exception(reader.GetString(3))
						});
					}
				}
				return msgs.ToArray();
			}
		}

		public int GetCountInBatches()
		{
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT COUNT(*) FROM OutgoingMessages WHERE BatchId IS NOT NULL";
				return (int) cmd.ExecuteScalar();
			}
		}

		public int GetCountInDeadLetters()
		{
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT COUNT(*) FROM FailedMessages";
				return (int)cmd.ExecuteScalar();
			}
		}

		public int GetCountInActiveMessages()
		{
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT COUNT(*) FROM OutgoingMessages WHERE BatchId IS NULL";
				return (int)cmd.ExecuteScalar();
			}
		}

		public void Dispose()
		{
		}

		public QueueTransportMessage GetLatestMessage()
		{
			return (
			       	from m in GetTransportMessages()
			       	orderby m.SendAt descending
			       	select m
			       ).FirstOrDefault();
		}
	}
}