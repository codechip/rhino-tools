namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Scheduler
    {
        //I am waiting for the bug about this not being refreshed with hot swappable CPUs
        private readonly int cachedProcessorCount = Environment.ProcessorCount;
        private readonly AbstractTask idleTask = new YieldOnIdleTask();
        private readonly LinkedList<SchedulePair> queuedTasks = new LinkedList<SchedulePair>();
        private readonly LinkedList<AbstractTask> executingTasks = new LinkedList<AbstractTask>();
        private readonly object syncLock = new object();
        private readonly LinkedList<Thread> threads = new LinkedList<Thread>();
        private int currentRunningProcessCount = 0;
        private ScheduleStatus status;
        private int threadCount;
        private int totalTasks;

        public int TotalTasks
        {
            get { return totalTasks; }
        }

        public ScheduleStatus Status
        {
            get { return status; }
            private set { status = value; }
        }

        public Future Schedule(AbstractTask task)
        {
            task.Initialize(this);
            return Reschedule(task, delegate { return true; });
        }

        public Future Reschedule(AbstractTask task, Condition condition)
        {
            lock (syncLock)
            {
                totalTasks += 1;
                if (task.ExecutionState == ExecutionState.Running)
                {
                    Interlocked.Decrement(ref currentRunningProcessCount);
                    executingTasks.Remove(task);
                }
                task.ExecutionState = ExecutionState.Scheduled;
                queuedTasks.AddLast(new SchedulePair(task, condition));
                Monitor.PulseAll(syncLock);
            }
            return task.Completed;
        }

        public void Execute()
        {
            while (true)
            {
                if (TrySchedule(RunInSeparateThread, ThreadingOptions.WillCreateNewThread) == false)
                    break;
            }
        }

        public void ExecuteTaskInSameThread(AbstractTask task)
        {
            task.Continue();
        }

        public void RunInSeparateThread(AbstractTask task)
        {
            int currentThreadId = Interlocked.Increment(ref threadCount);
            Thread thread = new Thread(delegate(object state)
            {
                try
                {
                    task.Continue();//execute once 
                    //now we will reuse this thread to execute additional schedling
                    while (true)
                    {
                        if (TrySchedule(ExecuteTaskInSameThread, ThreadingOptions.ReuseCurrentThread) == false)
                            break;
                    }
                }
                finally
                {
                    LinkedListNode<Thread> node = (LinkedListNode<Thread>) state;
                    threads.Remove(node);
                }
            });
            LinkedListNode<Thread> last = threads.AddLast(thread);
            thread.Name = "Scheduler Thread #" + currentThreadId;
            thread.Start(last);
        }

        public void ScheduleFirst(AbstractTask task)
        {
            lock(syncLock)
            {
                task.ExecutionState = ExecutionState.Scheduled;
                queuedTasks.AddFirst(new SchedulePair(task, delegate { return true; }));
                Monitor.PulseAll(syncLock);
            }
        }

        public bool TrySchedule(Action<AbstractTask> executeTask, ThreadingOptions threadingOptions)
        {
            AbstractTask task = null;
            lock (syncLock)
            {
                if (queuedTasks.Count == 0 && currentRunningProcessCount == 0)
                    return false;
                bool? result = TryGetTaskToRun(ref task, threadingOptions);
                if (result.HasValue)
                    return result.Value;

                //limit to the required concurrency level
                
                //note that here we make a copy of the value, to avoid issues
                //with using a cache for it, thus missing a value change.
                int tmpCurrentRunningProcesses = currentRunningProcessCount;
                while (tmpCurrentRunningProcesses >= cachedProcessorCount)
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
                executingTasks.AddLast(task);
                if (totalTasks%100 == 0)
                {
                    Console.WriteLine("Running: {0}, queued: {1}, Total: {2}", currentRunningProcessCount,
                                      queuedTasks.Count, totalTasks);
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
            LinkedListNode<SchedulePair> node = queuedTasks.First;
            while (node != null)
            {
                SchedulePair pair = node.Value;
                if (pair.Condition())
                {
                    task = pair.Task;
                    queuedTasks.Remove(node);
                    break;
                }
                pair.CannotRunCount += 1;
                //this is probably a process that is waiting for other processes
                //move it to the end of the list so we don't have to iterate over it all
                //the time
                LinkedListNode<SchedulePair> prev = node;
                node = node.Next;
                if (pair.CannotRunCount == 3 && Status != ScheduleStatus.Idle)
                {
                    pair.CannotRunCount = -1; //give it a bit of a boost for the next time
                    queuedTasks.Remove(prev);
                    queuedTasks.AddLast(prev);
                }
            }
            if (task == null) // no tasks to run
            {
                //nothing runs, and there are no tasks that we _can_ run
                // we are either deadlocked or waiting for an external resource
                // we will let the idle task decide what to do.
                if (currentRunningProcessCount == 0)
                {
                    Status = ScheduleStatus.Idle;
                    task = idleTask;
                    return true;
                }
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
                executingTasks.Remove(task);
                Monitor.PulseAll(syncLock);
            }
        }

        #region Nested type: SchedulePair

        public class SchedulePair
        {
            public int CannotRunCount = 0;
            public Condition Condition;
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