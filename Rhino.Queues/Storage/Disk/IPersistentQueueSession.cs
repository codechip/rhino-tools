namespace Rhino.Queues.Storage.Disk
{
	using System;

	public interface IPersistentQueueSession : IDisposable
	{
		void Enqueue(byte[] data);
		byte[] Dequeue();
	}
}