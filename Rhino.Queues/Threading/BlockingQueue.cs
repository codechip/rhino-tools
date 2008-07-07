using System.Collections.Generic;
using System.Threading;

namespace Rhino.Queues.Threading
{
	public class BlockingQueue<T> : IBlockingQueue<T>
	{
		private readonly LinkedList<T> queue = new LinkedList<T>();
		private readonly object locker = new object();

		private bool active = true;

		public void Enqueue(T o)
		{
			lock(locker)
			{
				queue.AddFirst(o);
				Monitor.Pulse(locker);
			}
		}

		public T Dequeue()
		{
			lock(locker)
			{
				while (queue.Count == 0 && active)
					Monitor.Wait(locker);
				if(active==false)
					return default(T);
				var value = queue.Last.Value;
				queue.RemoveLast();
				return value;
			}
		}

		public void Dispose()
		{
			lock (locker)
			{
				queue.Clear();
				active = false;
				Monitor.PulseAll(locker);
			}
		}
	}
}