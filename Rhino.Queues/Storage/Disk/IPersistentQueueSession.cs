namespace Rhino.Queues.Storage.Disk
{
	using System;

	public interface IPersistentQueueSession : IDisposable
	{
		bool IsUsable { get; }
		void Enqueue(byte[] data);
		byte[] Dequeue();
		byte[] ReversibleDequeue(out Action reverse);
	}
}