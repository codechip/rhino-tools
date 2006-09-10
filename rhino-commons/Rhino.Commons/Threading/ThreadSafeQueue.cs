using System.Collections;
using System.Threading;

namespace Rhino.Commons
{
	public interface IQueue<T>
	{
		void Enqueue(T o);
		T Dequeue();
	}

	public class ThreadSafeQueue<T> : IQueue<T>
	{
		private Queue q = new Queue();

		public ThreadSafeQueue()
		{
		}

		public void Enqueue(T o)
		{
			lock (this)
			{
				q.Enqueue(o);
				Monitor.Pulse(this);
			}
		}

		public T Dequeue()
		{
			lock (this)
			{
				while (q.Count == 0)
				{
					Monitor.Wait(this);
				}
				return (T)q.Dequeue();
			}
		}
	    
	    public bool TryDequeue(out T item)
	    {
            lock (this)
            {
                if (q.Count == 0)
                {
                    item = default(T);
                    return false;
                }
                item = (T) q.Dequeue();
                return true;
            }
	    }
	}

}