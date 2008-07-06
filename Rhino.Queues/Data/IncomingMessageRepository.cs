using System;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Data;

namespace Rhino.Queues.Data
{
	public class IncomingMessageRepository : DataAccessMixin, IIncomingMessageRepository
	{
		private readonly string name;

		public string Name
		{
			get { return name; }
		}

		public IncomingMessageRepository(string name, string path)
			: base(name, path)
		{
			this.name = name;
		}

		public void Save(params QueueMessage[] msgs)
		{
			Transaction(cmd =>
			{
				foreach (var msg in msgs)
				{
					cmd.Parameters.Clear();

					cmd.CommandText = Queries.GetCountOfArrivedMessagesById;
					cmd.Parameters.Add("@Id", msg.Id.ToString());
					var count = (int)cmd.ExecuteScalar();
					if (count != 0)
						return;

					cmd.CommandText = Queries.TrackNewIncomingMessage;
					cmd.Parameters.Add("@Id", msg.Id.ToString());

					cmd.ExecuteNonQuery();

					cmd.CommandText = Queries.InsertMessageToIncomingQueue;
					cmd.Parameters.Add("@Data", Serialize(msg));
					cmd.Parameters.Add("@InsertedAt", SystemTime.Now());
					cmd.ExecuteNonQuery();
				}
			});
		}

		public void PurgeAllMessages()
		{
			Transaction(cmd =>
			{
				cmd.CommandText = Queries.PurgeAllMessagesFromIncoming;
				cmd.ExecuteNonQuery();
			});
		}

		public QueueMessage GetEarliestMessage()
		{
			byte[] data = null;
			bool done = false;
			while (done == false)
			{
				Transaction(cmd =>
				{
					cmd.CommandText = Queries.GetEarliestMessageFromIncomingQueue;
					string id;
					using (var reader = cmd.ExecuteReader())
					{
						if (reader.Read() == false)
						{
							done = true;
							return;
						}
						id = reader.GetString(0);
						data = (byte[])reader[1];
					}
					cmd.CommandText = Queries.DeleteMessageFromIncomingQueue;
					cmd.Parameters.Add("@Id", id);
					try
					{
						var rowAffected = cmd.ExecuteNonQuery();
						// someone else already grabbed and deleted this row, 
						// so we will try again with another one
						if (rowAffected != 1)
							return; // same as continue in this case} 
					}
					catch (FbException e)
					{
						// yuck! it would have been better to compare the error code
						// but FB doesn't exposes it
						if (e.Message == "cannot update erased record")
						{
							return;// same as continue
						}
					}
					done = true;// same as break from the loop
				});
			}
			if (data == null)
				return null;
			return Deserialize(data);
		}
	}
}