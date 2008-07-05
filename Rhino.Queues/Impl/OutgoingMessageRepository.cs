using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BerkeleyDb;

namespace Rhino.Queues.Impl
{
	public class OutgoingMessageRepository : IOutgoingMessageRepository
	{
		private readonly string name;
		private readonly string path;

		public OutgoingMessageRepository(string name, string path)
		{
			this.name = name;
			this.path = path;
		}

		public string Name
		{
			get { return name; }
		}

		public event Action NewMessageStored = delegate { };

		public void Save(Uri destination, QueueMessage msg)
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				queue.AppendAssociation(tree, new QueueTransportMessage
				{
                    SendAt = SystemTime.Now(),
					Destination = destination,
					Message = msg
				});
				tx.Commit();
			}
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
			var batch = new MessageBatch();

			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			using (var batches = env.OpenTree(name + ".batches"))
			{
				var msgs = new List<QueueTransportMessage>();
				foreach (var msg in queue.SelectAndConsumeFromAssociation<QueueTransportMessage>(tree,
						m => m.SendAt <= SystemTime.Now()))
				{
					msgs.Add(msg);
					if (msgs.Count == 100)
						break;
				}

				var destBatches = from msg in msgs
								  group msg by msg.Destination
									  into g
									  select new SingleDestinationMessageBatch
									  {
										  BatchId = batch.Id,
										  Destination = g.Key,
										  Messages = g.Select(m => m.Message).ToArray()
									  };

				batch.DestinationBatches = destBatches.ToArray();

				foreach (var messageBatch in destBatches)
				{
					batches.Put(new MessageBatchKey(messageBatch), messageBatch);
				}

				tx.Commit();
			}
			return batch;
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
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			using (var batches = env.OpenTree(name + ".batches"))
			using (var deadLetters = env.OpenTree(name + ".deadLetters"))
			{
				var key = new MessageBatchKey(batchId, destination);
				var batch = (SingleDestinationMessageBatch)batches.Get(key);
				if(batch==null)
					return;
				batches.Delete(key);
				foreach (var message in batch.Messages)
				{
					message.BatchId = null;
					message.FailureCount++;
					var secondsToAdd = Math.Min(Math.Pow(message.FailureCount, 2), 300);
					DateTime timeToSend = SystemTime.Now().AddSeconds(secondsToAdd);
					if (message.FailureCount <= maxFailureCount)
					{
						queue.AppendAssociation(tree, new QueueTransportMessage
						{
							Destination = destination,
                            Message = message,
                            SendAt = timeToSend
						});
					}
					else
					{
						deadLetters.Put(message.Id, new FailedQueueMessage
						{
							Destination = destination,
                            Exception = exception,
                            FinalFailureAt = SystemTime.Now(),
                            Message = message
						});
					}
				}
				tx.Commit();
			}
		}

		/// <summary>
		/// Resets all batches, it is expected that this will happen only on system startup,
		/// to recover from possible crashes that left batches orhpaned
		/// </summary>
		public void ResetAllBatches()
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			using (var batches = env.OpenTree(name + ".batches"))
			{
				foreach (DictionaryEntry de in batches.SelectAndConsume())
				{
					var batch = (SingleDestinationMessageBatch)de.Value;
					foreach (var message in batch.Messages)
					{
						message.BatchId = null;
						queue.AppendAssociation(tree,new QueueTransportMessage
						{
							Destination = batch.Destination,
                            Message = message,
                            SendAt = SystemTime.Now()
						});
					}
				}
				tx.Commit();
			}
		}

		public void RemoveSuccessfulBatch(Guid batchId, Uri destination)
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var batches = env.OpenTree(name + ".batches"))
			{
				batches.Delete(new MessageBatchKey(batchId, destination));
				tx.Commit();
			}
		}

		public void PurgeAllMessages()
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			using (var batches = env.OpenTree(name + ".batches"))
			using (var deadLetters = env.OpenTree(name + ".deadLetters"))
			{
				tree.Truncate();
				queue.Truncate();
				batches.Truncate();
				deadLetters.Truncate();
				tx.Commit();
			}
		}
	}
}