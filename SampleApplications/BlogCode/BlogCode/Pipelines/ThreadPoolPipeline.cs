namespace pipelines.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class ThreadPoolPipeline<T>
    {
        private readonly List<IOperation<T>> operations = new List<IOperation<T>>();

        public ThreadPoolPipeline<T> Register(IOperation<T> operation)
        {
            operations.Add(operation);
            return this;
        }

        public void Execute()
        {
            IEnumerable<T> current = new List<T>();
            foreach (IOperation<T> operation in operations)
            {
                IEnumerable<T> execute = operation.Execute(current);
                current = StartConsuming(execute);
            }
            IEnumerator<T> enumerator = current.GetEnumerator();
            while (enumerator.MoveNext()) ;
        }

        private ThreadSafeEnumerator<T> StartConsuming(IEnumerable<T> enumerable)
        {
            ThreadSafeEnumerator<T> threadSafeEnumerator = new ThreadSafeEnumerator<T>();
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    foreach (T t in enumerable)
                    {
                        threadSafeEnumerator.AddItem(t);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    threadSafeEnumerator.MarkAsFinished();
                }
            });
            return threadSafeEnumerator;
        }
    }
}