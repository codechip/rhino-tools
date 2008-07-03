namespace Rhino.Queues.Workers
{
	public interface IQueueWorker
	{
		void Run();
		void NewMessageStored();
		void Stop();
	}
}