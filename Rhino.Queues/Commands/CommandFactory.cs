using Rhino.Queues.Commands;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Commands
{
	public class CommandFactory : ICommandFactory
	{
		private readonly IQueueFactoryImpl queueFactory;

		public CommandFactory(IQueueFactoryImpl queueFactory)
		{
			this.queueFactory = queueFactory;
		}

		public virtual ICommand CreateCommand(SingleDestinationMessageBatch batch)
		{
			ICommand cmd;
			if (IsLocal(batch))
			{
				cmd = new AcceptMessagesCommand(queueFactory, batch);
			}
			else
			{
				cmd = new SendToRemoteServerCommand(queueFactory, batch);
			}
			return cmd;
		}

		private bool IsLocal(SingleDestinationMessageBatch batch)
		{
			return queueFactory.IsLocal(batch.Destination);
		}
		
	}
}