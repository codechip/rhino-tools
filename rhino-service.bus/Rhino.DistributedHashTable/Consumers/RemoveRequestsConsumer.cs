namespace Rhino.DistributedHashTable.Consumers
{
	using Messages;
	using ServiceBus;

	public class RemoveRequestsConsumer : ConsumerOf<RemoveRequests>
	{
		private readonly IDistributedHashTable distributedHashTable;

		public RemoveRequestsConsumer(IDistributedHashTable distributedHashTable)
		{
			this.distributedHashTable = distributedHashTable;
		}

		public void Consume(RemoveRequests message)
		{
			distributedHashTable.Remove(distributedHashTable.Url, message.Requests);
		}
	}
}