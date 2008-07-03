namespace Rhino.Queues
{
	public interface IQueuePhysicalStorage
	{
		void CreateQueue(string queueName);
		string[] GetQueueNames();
	}
}