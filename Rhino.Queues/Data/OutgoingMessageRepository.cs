using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using Rhino.Queues.Data;

namespace Rhino.Queues.Data
{
	public class OutgoingMessageRepository : DataAccessMixin, IOutgoingMessageRepository
	{
		private readonly string name;

		public OutgoingMessageRepository(string name, string path)
			:base(name, path)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		public event Action NewMessageStored = delegate { };

		public void Save(Uri destination, QueueMessage msg)
		{
			Transaction(cmd =>
			{
				cmd.CommandText = Queries.InsertMessageToOutgoingQueue;
				cmd.Parameters.Add("@Id", msg.Id.ToString());
				cmd.Parameters.Add("@Destination", destination.ToString());
				cmd.Parameters.Add("@Data", Serialize(msg));
				cmd.Parameters.Add("@InsertedAt", SystemTime.Now());
				cmd.Parameters.Add("@SendAt", SystemTime.Now());
				cmd.ExecuteNonQuery();
			});
			// must be after the change was committed
			NewMessageStored();
		}

		/// <summary>
		/// The implementation of this method is a bit tricky.
		/// First, we consume all messages that are not in a batch in our newly created batch
		/// but only if they are elgibile to send.
		/// We format the results for easy consumtion and then write the batch, so we can track
		/// its send progress and if it was successful or not.
		/// We are doing this in two stages to ensure that we two transactions cannot get the 
		/// same message.
		/// </summary>
		/// <returns></returns>
		public MessageBatch GetBatchOfMessagesToSend()
		{
			MessageBatch batch = null;
			Transaction(cmd =>
			{
				batch = new MessageBatch();

				

				var destToMsgs = new Dictionary<Uri, List<QueueMessage>>();
				var ids = new List<string>();
				cmd.CommandText = Queries.SelectMessagesFromOutgoing;
				cmd.Parameters.Add("@CurrentTime", SystemTime.Now());
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						ids.Add(reader.GetString(0));
						var destination = reader.GetString(1);
						List<QueueMessage> messages;
						var destinationUri = new Uri(destination);
						if (destToMsgs.TryGetValue(destinationUri, out messages) == false)
							destToMsgs[destinationUri] = messages = new List<QueueMessage>();
						var data = (byte[])reader[2];
						messages.Add(Deserialize(data));
					}
				}

				cmd.Parameters.Clear();
				cmd.CommandText = Queries.UpdateBatchId;
				cmd.Parameters.Add("@BatchId", FbDbType.Char);
				cmd.Parameters.Add("@Id", FbDbType.Char);
				foreach (var msgId in ids)
				{
					cmd.Parameters["@BatchId"].Value = batch.Id;
					cmd.Parameters["@Id"].Value = msgId;
					cmd.ExecuteNonQuery();
				}

				var destBatches = new List<SingleDestinationMessageBatch>();
				foreach (var msg in destToMsgs)
				{
					destBatches.Add(new SingleDestinationMessageBatch
					{
						BatchId = batch.Id,
						Destination = msg.Key,
						Messages = msg.Value.ToArray()
					});
				}
				batch.DestinationBatches = destBatches.ToArray();
			});
			return batch ?? new MessageBatch();
		}

		/// <summary>
		/// Resets the batch id for the messages that match the destination
		/// and batch id.
		/// Usually used if there was a failure sending them to that destination
		/// </summary>
		/// <param name="batchId">The batch id.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="maxFailureCount">The maximum amount a message is allowed to fail before it is considered dead</param>
		/// <param name="exception">The exception that caused this batch failure</param>
		public void ReturnedFailedBatchToQueue(Guid batchId, Uri destination, 
		                                       int maxFailureCount, Exception exception)
		{
			Transaction(cmd =>
			{
				cmd.CommandText = Queries.SelectFailureCountAndTime;
				cmd.Parameters.Add("@BatchId", batchId.ToString());
				cmd.Parameters.Add("@Destination", destination.ToString());
				var failuresById = new Dictionary<string, int>();
				using(var reader = cmd.ExecuteReader())
				{
					while(reader.Read())
					{
						failuresById.Add(reader.GetString(0), reader.GetInt32(1));
					}
				}
				cmd.Parameters.Clear();

				cmd.CommandText = Queries.UpdateFailureCountAndTimeToSend;
				cmd.Parameters.Add("@Id", FbDbType.Char);
				cmd.Parameters.Add("@FailureCount", FbDbType.Integer);
				cmd.Parameters.Add("@SendAt", FbDbType.TimeStamp);
				
				foreach (var pair in failuresById)
				{
					cmd.Parameters["@Id"].Value = pair.Key;
					var updatedFailureCount = pair.Value + 1;
					cmd.Parameters["@FailureCount"].Value = updatedFailureCount;
					var secondsToAdd = Math.Min(Math.Pow(updatedFailureCount, 2), 300);
					cmd.Parameters["@SendAt"].Value = SystemTime.Now().AddSeconds(secondsToAdd);
					cmd.ExecuteNonQuery();
				}

				cmd.Parameters.Clear();

				cmd.CommandText = Queries.MoveAllMessagesInBatchWithFailureCountToFAiledMessages_Part1;
				cmd.Parameters.Add("@BatchId", batchId.ToString());
				cmd.Parameters.Add("@Destination", destination.ToString());
				cmd.Parameters.Add("@CurrentTime", SystemTime.Now());
				cmd.Parameters.Add("@MaxNumberOfFailures", maxFailureCount);
				cmd.Parameters.Add("@LastException", exception.ToString());

				cmd.ExecuteNonQuery();
				cmd.CommandText = Queries.MoveAllMessagesInBatchWithFailureCountToFAiledMessages_Part2;
				cmd.ExecuteNonQuery();
				
				cmd.CommandText = Queries.ResetBatchIdForOutgoingQueueByIdAndDestination;
				cmd.Parameters.Add("@BatchId", batchId.ToByteArray());
				cmd.Parameters.Add("@Destination", destination.ToString());
				cmd.ExecuteNonQuery();
			});
		}

		/// <summary>
		/// Resets all batches, it is expected that this will happen only on system startup,
		/// to recover from possible crashes that left batches orhpaned
		/// </summary>
		public void ResetAllBatches()
		{
			Transaction(cmd =>
			{
				cmd.CommandText = Queries.ResetAllBatchIdForOutgoingQueue;
				cmd.ExecuteNonQuery();
			});
		}

		public void RemoveSuccessfulBatch(Guid batchId, Uri destination)
		{
			Transaction(cmd =>
			{
				cmd.CommandText = Queries.DeleteSuccessfulBatch;
				cmd.Parameters.Add("@BatchId", batchId.ToString());
				cmd.Parameters.Add("@Destination", destination.ToString());
				cmd.ExecuteNonQuery();
			});
		}

		public void PurgeAllMessages()
		{
			Transaction(cmd =>
			{
				foreach (var command in Queries.PurgeAllMessagesFromOutgoing.Split(';'))
				{
					if(command.Trim().Length==0)
						continue;
					
					cmd.CommandText = command;
					cmd.ExecuteNonQuery();
				}
			});
		}
	}
}