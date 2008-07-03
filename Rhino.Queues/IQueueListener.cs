namespace Rhino.Queues
{
	public interface IQueueListener
	{
		void Stop();
		void Start(IQueueFactory queueFactory);
	}
}