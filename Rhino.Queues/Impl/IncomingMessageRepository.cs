using System;

namespace Rhino.Queues.Impl
{
	public class IncomingMessageRepository : MessageRepositoryBase, IIncomingMessageRepository
	{
		public IncomingMessageRepository(string name, string directory):base(name, directory)
		{
			
		}

		public void Save(QueueMessage msg)
		{
			Transaction(() =>
			{
				var serialized = Serialize(msg);

				cmd.CommandText = Queries.InsertMessageToIncomingQueue;
				AddParameter("@Id", msg.Id);
				AddParameter("@Data", serialized);
				AddParameter("@InsertedAt", SystemTime.Now());
				cmd.ExecuteNonQuery();
			});
		}

		public void PurgeAllMessages()
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.PurgeAllMessagesFromIncoming;
				cmd.ExecuteNonQuery();
			});
		}

		public QueueMessage GetEarliestMessage()
		{
			byte[] data = null;
			Transaction(() =>
			{
				cmd.CommandText = Queries.GetEarliestMessageFromIncomingQueue;
				while (true)
				{
					Guid id;
					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read() == false)
							return;
						id = (Guid) reader[0];
						data = (byte[]) reader[1];
					}
					using (var delMsgCmd = connection.CreateCommand())
					{
						delMsgCmd.Transaction = transaction;
						delMsgCmd.CommandText = Queries.DeleteMessageFromIncomingQueue;
						AddParameter(delMsgCmd, "Id", id);
						var rowAffected = delMsgCmd.ExecuteNonQuery();
						// someone else already grabbed and deleted this row, 
						// so we will try again with another one
						if (rowAffected != 1)
							continue;
						break;
					}
				}
			});
			if (data==null)
				return null;
			return Deserialize(data);

		}

		protected override string GetQueryToGenerateDatabase()
		{
			return Queries.CreateTablesForIncomingQueue;
		}
	}
}