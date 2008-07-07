using System;

namespace Rhino.Queues.Threading
{
	public interface IBlockingQueue<T> : IDisposable
	{
		void Enqueue(T o);
		T Dequeue();
	}
}