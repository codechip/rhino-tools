namespace Rhino.Queues
{
	public interface IQueuePhysicalStorage
	{
		void CreateInputQueue(string queueName);
		string[] GetQueueNames();
	}
}