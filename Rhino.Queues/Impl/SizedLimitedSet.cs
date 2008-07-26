namespace Rhino.Queues.Impl
{
	using System;
	using System.Collections.Generic;

	public class SizedLimitedSet<T>
	{
		readonly HashSet<T> set = new HashSet<T>();
		readonly LinkedList<T> orderedItems = new LinkedList<T>();

		private readonly int maxCount;

		public int Count
		{
			get { return set.Count; }
		}

		public SizedLimitedSet(int maxCount)
		{
			this.maxCount = maxCount;
			if (maxCount <= 1)
				throw new ArgumentException("maxCount cannot be less than 1");
		}

		public void Add(T item)
		{
			orderedItems.AddFirst(item);
			set.Add(item);
			if (set.Count <= maxCount) 
				return;
			// clear oldest item
			var last = orderedItems.Last.Value;
			set.Remove(last);
			orderedItems.RemoveLast();
		}

		public bool Exists(T item)
		{
			return set.Contains(item);
		}
	}
}