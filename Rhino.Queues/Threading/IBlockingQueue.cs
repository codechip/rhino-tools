using System;

namespace Rhino.Queues.Threading
{
	public interface IBlockingQueue<T> : IDisposable
	{
		void Enqueue(T o);
		bool Dequeue(TimeSpan timeToWait, out T t);
	}
}