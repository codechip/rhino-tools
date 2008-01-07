namespace pipelines.Pipelines
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    public class ThreadSafeEnumerator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private bool active = true;
        private readonly Queue<T> cached = new Queue<T>();
        private T current;

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        public T Current
        {
            get { return current; }
        }

        public void Dispose()
        {
            cached.Clear();
        }

        public bool MoveNext()
        {
            lock (cached)
            {
                while (cached.Count == 0 && active)
                    Monitor.Wait(cached);

                if (active == false && cached.Count == 0)
                    return false;

                current = cached.Dequeue();

                return true;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void AddItem(T item)
        {
            lock (cached)
            {
                cached.Enqueue(item);
                Monitor.Pulse(cached);
            }
        }

        public void MarkAsFinished()
        {
            lock (cached)
            {
                active = false;
                Monitor.Pulse(cached);
            }

        }
    }
}