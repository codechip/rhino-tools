namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Scheduler
    {
        private int totalTasks;
        public class SchedulePair
        {
            public AbstractTask Task;
            public Condition Condition;

            public SchedulePair(AbstractTask task, Condition condition)
            {
                Task = task;
                Condition = condition;
            }
        }

        private readonly object syncLock = new object();
        private readonly LinkedList<SchedulePair> queqedTasks = new LinkedList<SchedulePair>();
        private int currentRunningProcessCount = 0;

        public Future Schedule(AbstractTask task)
        {
            task.Initialize(this);
            lock (syncLock)
            {
                totalTasks += 1;
                queqedTasks.AddLast(new SchedulePair(task, delegate { return true; }));
                Monitor.PulseAll(syncLock);
            }
            return task.Completed;
        }

        public void Schedule(AbstractTask task, Condition condition)
        {
            lock (syncLock)
            {
                totalTasks += 1;
                queqedTasks.AddLast(new SchedulePair(task, condition));
                Monitor.PulseAll(syncLock);
            }
        }

        public void Execute()
        {
            while (true)
            {
                if (TrySchedule(ExecuteInThreadPool, ThreadingOptions.WillCreateNewThread) == false)
                    break;
            }
        }

        public void ExecuteInThreadPool(AbstractTask task)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    SchedulingOptions.NestedStackCount = 0;
                    task.Continue();
                }
                finally
                {
                    SchedulingOptions.NestedStackCount = 0;
                }
            });

        }

        public bool TrySchedule(Action<AbstractTask> executeTask, ThreadingOptions threadingOptions)
        {
            AbstractTask task = null;
            lock (syncLock)
            {
                if (queqedTasks.Count == 0 && currentRunningProcessCount == 0)
                    return false;
                bool? result = TryGetTaskToRun(ref task, threadingOptions);
                if (result.HasValue)
                    return result.Value;
                //we want to avoid overwhelming the thread pool
                int maxRunningTasks = Environment.ProcessorCount;

                while (currentRunningProcessCount >= maxRunningTasks)
                {
                    if (threadingOptions == ThreadingOptions.WillCreateNewThread)
                    {
                        Monitor.Wait(syncLock);
                    }
                    else if (threadingOptions != ThreadingOptions.ReuseCurrentThread)
                    {
                        return false;
                    }
                    else//if we are reusing a thread, we can safely go "above" the maxRunnintTask limit
                    {
                        break;
                    }
                }
                if (threadingOptions != ThreadingOptions.ReuseCurrentThread)
                    Interlocked.Increment(ref currentRunningProcessCount);
                if (totalTasks % 100 == 0)
                {
                    System.Console.WriteLine("Running: {0}, queued: {1}, Total: {2}", currentRunningProcessCount, queqedTasks.Count, totalTasks);
                }
            }
            executeTask(task);
            return true;
        }
        /// <summary>
        /// This must run under the lock!
        /// </summary>
        private bool? TryGetTaskToRun(ref AbstractTask task, ThreadingOptions threadingOptions)
        {
            SchedulePair pairToRemove = null;
            LinkedListNode<SchedulePair> node = queqedTasks.First;
            while (node != null)
            {
                SchedulePair pair = node.Value;
                if (pair.Condition())
                {
                    task = pair.Task;
                    pairToRemove = pair;
                    break;
                }
            }
            if (pairToRemove != null)
            {
                queqedTasks.Remove(node);
            }
            if (task == null)// no tasks to run
            {
                if (currentRunningProcessCount == 0)//nothing runs, we are probably deadlocked
                    throw new DeadLockException();
                if (threadingOptions == ThreadingOptions.WillCreateNewThread)
                {
                    Monitor.Wait(syncLock); // wait for a change
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return null;
        }

        public void Completed(AbstractTask task)
        {
            lock (syncLock)
            {
                if (SchedulingOptions.NestedStackCount == 0)
                    Interlocked.Decrement(ref currentRunningProcessCount);
                Monitor.PulseAll(syncLock);
            }
        }
    }
}