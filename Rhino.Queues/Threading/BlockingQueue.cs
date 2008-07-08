using System;
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
			lock (locker)
			{
				queue.AddFirst(o);
				Monitor.Pulse(locker);
			}
		}

		public bool Dequeue(TimeSpan timeToWait, out T t)
		{
			lock (locker)
			{
				while (queue.Count == 0 && active)
				{
					if (Monitor.Wait(locker, timeToWait)==false)
					{
						t = default(T); 
						return false;
					}
				}
				if (active == false)
				{
					t = default(T);
					return false;
				}
				var value = queue.Last.Value;
				queue.RemoveLast();
				t = value;
				return true;
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