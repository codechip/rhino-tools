using System.Collections;
using System.Threading;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
	public interface IQueue
	{
		void Enqueue(object o);
		object Dequeue();
	}

	public class ThreadSafeQueue : IQueue
	{
		private Queue q = new Queue();

		public ThreadSafeQueue()
		{
		}

		public void Enqueue(object o)
		{
			lock (this)
			{
				q.Enqueue(o);
				Monitor.Pulse(this);
			}
		}

		public object Dequeue()
		{
			lock (this)
			{
				while (q.Count == 0)
				{
					Monitor.Wait(this);
				}
				return q.Dequeue();
			}
		}
	}

}