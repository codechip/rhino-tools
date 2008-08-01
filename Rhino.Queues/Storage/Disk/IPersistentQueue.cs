namespace Rhino.Queues.Storage.Disk
{
	using System;

	public interface IPersistentQueue : IDisposable
	{
		IPersistentQueueSession OpenSession();
	}
}