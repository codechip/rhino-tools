using System;
using System.Collections.Generic;
using System.Data;

namespace Rhino.Queues.Impl
{
	public class OutgoingMessageRepository : MessageRepositoryBase, IOutgoingMessageRepository
	{
		private readonly string name;

		public OutgoingMessageRepository(string name, string directory)
			: base(name, directory)
		{
			this.name = name;
		}

		public OutgoingMessageRepository(string name) 
			: this(name, Environment.CurrentDirectory)
		{
		}

		public string Name
		{
			get{ return name; }
		}

		public event Action NewMessageStored = delegate { };

		public void Save(Uri destination, QueueMessage msg)
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.InsertMessageToOutgoingQueue;
				AddParameter("@Id", msg.Id);
				AddParameter("@Destination", destination.ToString());
				AddParameter("@Data", Serialize(msg));
				AddParameter("@InsertedAt", SystemTime.Now());
				AddParameter("@SendAt", SystemTime.Now());
				cmd.ExecuteNonQuery();
			});
			// must be after the change was committed
			NewMessageStored();
		}

		/// <summary>
		/// The implementation of this method is a bit tricky.
		/// First, we enlist all messages that are not in a batch in our newly created batch
		/// but only if they are elgibile to send.
		/// Then, we select all the messages in the current batch, and return them in a form
		/// that is more easily to process.
		/// We are doing this in two stages to ensure that we two transactions cannot get the 
		/// same message.
		/// </summary>
		/// <returns></returns>
		public MessageBatch GetBatchOfMessagesToSend()
		{
			var batch = new MessageBatch();
			Transaction(() =>
			{
				UpdateBatchIdForCurrentMessages(batch);

				var destToMsgs = HydrateMessageInCurrentBatch(batch);

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
			return batch;
		}

		private static void UpdateBatchIdForCurrentMessages(MessageBatch batch)
		{
			cmd.CommandText = Queries.UpdateBatchId;
			AddParameter("BatchId", batch.Id);
			AddParameter("CurrentTime", SystemTime.Now());
			cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
		}

		private Dictionary<Uri, List<QueueMessage>> HydrateMessageInCurrentBatch(MessageBatch batch)
		{
			var destToMsgs = new Dictionary<Uri, List<QueueMessage>>();
			cmd.CommandText = Queries.SelectMessagesFromOutgoing;
			AddParameter("BatchId", batch.Id);
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var destination = reader.GetString(0);
					List<QueueMessage> messages;
					var destinationUri = new Uri(destination);
					if (destToMsgs.TryGetValue(destinationUri, out messages) == false)
						destToMsgs[destinationUri] = messages = new List<QueueMessage>();
					var data = (byte[])reader[1];
					messages.Add(Deserialize(data));
				}
			}
			return destToMsgs;
		}

		protected override string GetQueryToGenerateDatabase()
		{
			return Queries.CreateTablesForOutgoingQueue;
		}

		/// <summary>
		/// Resets the batch id for the messages that match the destination
		/// and batch id.
		/// Usually used if there was a failure sending them to that destination
		/// </summary>
		/// <param name="batchId">The batch id.</param>
		/// <param name="destination">The destination.</param>
		public void ResetBatch(Guid batchId, Uri destination)
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.ResetBatchIdForOutgoingQueueByIdAndDestination;
				AddParameter("BatchId", batchId);
				AddParameter("Destination", destination.ToString());
				cmd.ExecuteNonQuery();
			});
		}

		/// <summary>
		/// Resets all batches, it is expected that this will happen only on system startup,
		/// to recover from possible crashes that left batches orhpaned
		/// </summary>
		public void ResetAllBatches()
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.ResetAllBatchIdForOutgoingQueue;
				cmd.ExecuteNonQuery();
			});
		}

		public void RemoveSuccessfulBatch(Guid batchId, Uri destination)
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.DeleteSuccessfulBatch;
				AddParameter("BatchId", batchId);
				AddParameter("Destination", destination.ToString());
				cmd.ExecuteNonQuery();
			});
		}

		/// <summary>
		/// Mark all messages in batch and destination as failures and increasing the
		/// time that they will be retried
		/// </summary>
		/// <param name="batchId">The batch id.</param>
		/// <param name="destination">The destination.</param>
		public void MarkAllInBatchAsFailed(Guid batchId, Uri destination)
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.UpdateFailureCountAndTimeToSend;
				AddParameter("BatchId", batchId);
				AddParameter("Destination", destination.ToString());
				cmd.ExecuteNonQuery();
			});
		}


		public void MoveUnderliverableMessagesToDeadLetterQueue(
			Guid batchId,
			Uri destination,
			int minNumberOfFailures,
			Exception lastException)
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.MoveAllMessagesInBatchWithFailureCountToFAiledMessages;
				AddParameter("BatchId", batchId);
				AddParameter("Destination", destination.ToString());
				AddParameter("CurrentTime", SystemTime.Now());
				AddParameter("MinNumberOfFailures", minNumberOfFailures);
				AddParameter("LastException", lastException.ToString());

				cmd.ExecuteNonQuery();
			});
		}

		public void PurgeAllMessages()
		{
			Transaction(() =>
			{
				cmd.CommandText = Queries.PurgeAllMessagesFromOutgoing;
				cmd.ExecuteNonQuery();
			});
		}
	}
}