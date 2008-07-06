using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Data
{
	public abstract class DataAccessMixin
	{
		private readonly string connectionString;

		protected DataAccessMixin(string name, string path)
		{
			connectionString = QueuePhysicalStorage.GetConnectionString(path, name);
		}

		public static byte[] Serialize(QueueMessage msg)
		{
			var stream = new MemoryStream();
			new BinaryFormatter().Serialize(stream, msg);
			return stream.ToArray();
		}

		public static QueueMessage Deserialize(byte[] data)
		{
			var stream = new MemoryStream(data);
			return (QueueMessage)new BinaryFormatter().Deserialize(stream);
		}

		protected void Transaction(Action<FbCommand> action)
		{
			using (var connection = new FbConnection(connectionString))
			using (var cmd = connection.CreateCommand())
			{
				connection.Open();
				using (var tx = connection.BeginTransaction(IsolationLevel.Serializable))
				{
					cmd.Transaction = tx;
					action(cmd);
					tx.Commit();
				}
			}
		}
	}
}