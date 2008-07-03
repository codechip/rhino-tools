using log4net;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Commands
{
	public class AcceptMessagesCommand : ICommand
	{
		private readonly ILog logger = LogManager.GetLogger(typeof (AcceptMessagesCommand));

		private readonly IQueueFactory queueFactory;
		private readonly SingleDestinationMessageBatch batch;

		public AcceptMessagesCommand(IQueueFactory queueFactory, 
			SingleDestinationMessageBatch batch)
		{
			this.queueFactory = queueFactory;
			this.batch = batch;
		}

		public void Execute()
		{
			var queue = (IQueueImpl)queueFactory.GetLocalQueue(batch.DestinationQueue);
			if(queue==null) 
			{
				logger.WarnFormat("Got a message batch #{0} for '{1}' from '{2}' - but there is not such queue. Discarding message batch.",
					batch.BatchId, 
					batch.Destination, 
					batch.Source);
				return;
			}
			logger.DebugFormat("Got {0} message(s) in batch #{1} for {2}", 
				batch.Messages.Length, batch.BatchId, batch.Destination);
			queue.AcceptMessages(batch.Messages);
		}
	}
}