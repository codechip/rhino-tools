namespace Rhino.Queues
{
	public interface IQueuePhysicalStorage
	{
		void CreateInputQueue(string queueName);
		string[] GetIncomingQueueNames();
		string[] GetOutgoingQueueNames();
	}
}