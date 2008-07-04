using System;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Commands
{
	public class SendToRemoteServerCommand : ICommand
	{
		private readonly ILog logger = LogManager.GetLogger(typeof(SendToRemoteServerCommand));

		public event Action Done = delegate { };

		private readonly IQueueImpl queue;
		private readonly SingleDestinationMessageBatch batch;
		private HttpWebRequest request;

		public SendToRemoteServerCommand(IQueueFactory queueFactory, SingleDestinationMessageBatch batch)
		{
			this.batch = batch;
			queue = (IQueueImpl)queueFactory.GetLocalQueue(batch.SourceQueue);
			if(queue==null)
			{
				throw new InvalidOperationException(
					string.Format("Message batch #{0} has been sent locally but has no matching queue '{1}' in the factory",
						batch.BatchId, batch.Source)
					);	
			}
		}

		public void Execute()
		{
			logger.DebugFormat("Starting to send batch #{0} to {1} with {2} messages",
				batch.BatchId, batch.Destination, batch.Messages.Length);

			var builder = new UriBuilder(batch.Destination) { Scheme = "http" };
			request = (HttpWebRequest)WebRequest.Create(builder.Uri);
			request.Method = "PUT";
			
			request.ConnectionGroupName = Guid.NewGuid().ToString();
			
			request.Headers["batch-id"] = batch.BatchId.ToString();
			using (var stream = request.GetRequestStream())
			{
				new BinaryFormatter().Serialize(stream, batch);
			}

			request.BeginGetResponse(OnGetResponse, null);
		}

		private void OnGetResponse(IAsyncResult ar)
		{
			try
			{
				try
				{
					// we don't care about the response
					var response = request.EndGetResponse(ar);
					response.Close();
				}
				catch (Exception e)
				{
					logger.Warn("Failed to send message batch #{0}" + batch.BatchId + " to '" + batch.Destination + "'.", e);

					try
					{
						queue.FailedToTransfer(batch, e);
					}
					catch (Exception exception)
					{
						logger.Error("Queue failed when moving batch to failed state. Trying again using fire & forget", exception);
						FireAndForget.Exceute(() => queue.FailedToTransfer(batch, e));
					}
					return;
				}
				logger.Debug("Successfully sent message batch #{0}" + batch.BatchId + " to '" + batch.Destination + "'.");
				try
				{
					queue.SuccessfullyTransfered(batch);
				}
				catch (Exception e)
				{
					logger.Error("Queue failed when moving batch to success state. Trying again using fire & forget", e);
					FireAndForget.Exceute(() => queue.SuccessfullyTransfered(batch));
				}
			}
			finally
			{
				Done();
			}
		}
	}
}