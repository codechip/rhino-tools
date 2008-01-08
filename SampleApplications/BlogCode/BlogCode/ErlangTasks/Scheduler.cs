namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Scheduler
    {
        //I am waiting for the bug about this not being refreshed with hot swappable CPUs
        private readonly int cachedProcessorCount = Environment.ProcessorCount;
        private readonly LinkedList<SchedulePair> queqedTasks = new LinkedList<SchedulePair>();
        private readonly object syncLock = new object();
        private int currentRunningProcessCount = 0;
        private int totalTasks;

        public int TotalTasks
        {
            get { return totalTasks; }
        }

        public Future Schedule(AbstractTask task)
        {
            task.Initialize(this);
            lock (syncLock)
            {
                totalTasks += 1;
                queqedTasks.AddLast(new SchedulePair(task, delegate { return true; }));
            }
            return task.Completed;
        }

        public void Schedule(AbstractTask task, Condition condition)
        {
            lock (syncLock)
            {
                totalTasks += 1;
                if (task.ExecutionState == ExecutionState.Running)
                {
                    Interlocked.Decrement(ref currentRunningProcessCount);
                }
                task.ExecutionState = ExecutionState.Scheduled;
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
            WaitCallback back = delegate
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
            };
            //back(null);
            ThreadPool.QueueUserWorkItem(back);
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

                //we want to avoid overwhelming the machine
                while (currentRunningProcessCount >= cachedProcessorCount)
                {
                    if (threadingOptions == ThreadingOptions.WillCreateNewThread)
                    {
                        Monitor.Wait(syncLock);
                    }
                    else if (threadingOptions != ThreadingOptions.ReuseCurrentThread)
                    {
                        return false;
                    }
                    else //if we are reusing a thread, we can safely go "above" the maxRunnintTask limit
                    {
                        break;
                    }
                }
                Interlocked.Increment(ref currentRunningProcessCount);
                if (totalTasks%100 == 0)
                {
                    Console.WriteLine("Running: {0}, queued: {1}, Total: {2}", currentRunningProcessCount,
                                      queqedTasks.Count, totalTasks);
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
            //this approach gives us an O(1) _removal_ cost from the list
            LinkedListNode<SchedulePair> node = queqedTasks.First;
            while (node != null)
            {
                SchedulePair pair = node.Value;
                if (pair.Condition())
                {
                    task = pair.Task;
                    queqedTasks.Remove(node);
                    break;
                }
                pair.CannotRunCount += 1;
                //this is probably a process that is waiting for other processes
                //move it to the end of the list so we don't have to iterate over it all
                //the time
                LinkedListNode<SchedulePair> prev = node;
                node = node.Next;
                if(pair.CannotRunCount==3)
                {
                    pair.CannotRunCount = 0;
                    queqedTasks.Remove(prev);
                    queqedTasks.AddLast(prev);
                }
            }
            if (task == null) // no tasks to run
            {
                if (currentRunningProcessCount == 0) //nothing runs, we are probably deadlocked
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
                Interlocked.Decrement(ref currentRunningProcessCount);
                Monitor.PulseAll(syncLock);
            }
        }

        #region Nested type: SchedulePair

        public class SchedulePair
        {
            public Condition Condition;
            public int CannotRunCount = 0;
            public AbstractTask Task;

            public SchedulePair(AbstractTask task, Condition condition)
            {
                Task = task;
                Condition = condition;
            }
        }

        #endregion
    }
}