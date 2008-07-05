using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Impl
{
	public class QueueListener : IQueueListener, IDisposable
	{
		private IQueueFactory queueFactory;
		private readonly BinaryFormatter formatter = new BinaryFormatter();
		private readonly HttpListener listener;
		private readonly ILog logger = LogManager.GetLogger(typeof(QueueListener));
		private bool disposed;

		public QueueListener(Uri url)
		{
			listener = new HttpListener();
			var prefix = new UriBuilder(url) { Scheme = "http" }.ToString();
			if (prefix.EndsWith("/") == false)
				prefix += "/";
			listener.Prefixes.Add(prefix);
		}

		public void Stop()
		{
			try
			{
				listener.Stop();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public void Start(IQueueFactory theQueueFactory)
		{
			queueFactory = theQueueFactory;
			listener.Start();
			listener.BeginGetContext(OnGetContext, null);
		}

		private void OnGetContext(IAsyncResult ar)
		{
			try
			{
				var context = listener.EndGetContext(ar);
				listener.BeginGetContext(OnGetContext, null);
				
				ProcessRequest(context);
			}
			catch (ObjectDisposedException)
			{
				// nothing to do here
			}
			catch (Exception e)
			{
				// we can't throw from here, because we are running in a thread context,
				// and it will kill the application
				logger.Error("Exception while processing request", e);
			}
		}

		private void ProcessRequest(HttpListenerContext context)
		{
			var batch = (SingleDestinationMessageBatch)formatter.Deserialize(
			                                           	context.Request.InputStream);

			var queue = (IQueueImpl)queueFactory.GetLocalQueue(batch.DestinationQueue);
			if (queue == null)
			{
				logger.WarnFormat("Got a batch #{0} of {1} messages from '{2}' for an invalid queue '{3}'",
				                  batch.BatchId, batch.Messages.Length, batch.Source, batch.Destination);
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				using (var sw = new StreamWriter(context.Response.OutputStream))
				{
					sw.WriteLine("Could not find queue: " + batch.Destination + " on this machine");
				}
				return;
			}
			logger.DebugFormat("Got a batch #{0} of {1} messages from '{2}' for queue '{3}'",
							   batch.BatchId, batch.Messages.Length, batch.Source, batch.Destination);
			queue.AcceptMessages(batch.Messages);

			context.Response.StatusCode = (int)HttpStatusCode.OK;
			using (var sw = new StreamWriter(context.Response.OutputStream))
			{
				sw.WriteLine("Successfully recieved {0} messages in batch #{1}",
				             batch.Messages.Length, batch.BatchId);
			}
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		~QueueListener()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (disposed)
				return;

			disposed = true;
			listener.Close();
		}
	}
}