using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Data;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	public class IncomingTestRepository : IDisposable
	{
		private readonly string connectionString;

		public IncomingTestRepository(string name)
		{
			connectionString = QueuePhysicalStorage.GetConnectionString(name, name);
		}

		public QueueMessage GetLatestMessage()
		{
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				cmd.CommandText = "SELECT FIRST 1 Data FROM IncomingMessages ORDER BY InsertedAt DESC";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						return DataAccessMixin.Deserialize((byte[])reader[0]);
					}
					return null;
				}
			}
		}

		public void Dispose()
		{
		}
	}
}