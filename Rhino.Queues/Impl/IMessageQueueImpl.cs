namespace Rhino.Queues.Impl
{
	public interface IMessageQueueImpl : IMessageQueue
	{
		void PutAll(TransportMessage[] msgs);
	}
}