namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Scheduler
    {
        [ThreadStatic]
        private static LinkedList<SchedulePair> currentThreadQueuedTasks;
        //I am waiting for the bug about this not being refreshed with hot swappable CPUs
        private readonly int cachedProcessorCount = Environment.ProcessorCount;
        private readonly AbstractTask idleTask = new YieldOnIdleTask();

        private readonly object syncLock = new object();
        private readonly LinkedList<Thread> threads = new LinkedList<Thread>();
        private int currentRunningProcessCount = 0;
        private ScheduleStatus status;
        private int threadCount;
        private int totalTasks;

        /// <summary>
        /// This contains the list of tasks to execute in the current queue
        /// </summary>
        private static LinkedList<SchedulePair> CurrentThreadQueuedTasks
        {
            get
            {
                if (currentThreadQueuedTasks == null)
                {
                    currentThreadQueuedTasks = new LinkedList<SchedulePair>();
                }
                return currentThreadQueuedTasks;
            }
        }

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
                }
                task.ExecutionState = ExecutionState.Scheduled;
                CurrentThreadQueuedTasks.AddLast(new SchedulePair(task, condition));
                Monitor.PulseAll(syncLock);
            }
            return task.Completed;
        }

        public void Execute()
        {
            for (int i = 0; i < cachedProcessorCount; i++)
            {
                SpinOffSchedulerThread();
            }
            while (true)
            {
                if (TrySchedule() == false)
                    break;
            }
        }

        public void SpinOffSchedulerThread()
        {
            int currentThreadId = Interlocked.Increment(ref threadCount);
            Thread thread = new Thread(delegate(object state)
            {
                try
                {
                    //now we will reuse this thread to execute additional schedling
                    while (true)
                    {
                        if (TrySchedule() == false)
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
            lock (syncLock)
            {
                task.ExecutionState = ExecutionState.Scheduled;
                CurrentThreadQueuedTasks.AddFirst(new SchedulePair(task, delegate { return true; }));
                Monitor.PulseAll(syncLock);
            }
        }

        public bool TrySchedule()
        {
            AbstractTask task = null;
            lock (syncLock)
            {
                if (CurrentThreadQueuedTasks.Count == 0 && currentRunningProcessCount == 0)
                    return false;
                bool? result = TryGetTaskToRun(ref task);
                if (result.HasValue)
                    return result.Value;

               
                Interlocked.Increment(ref currentRunningProcessCount);
                if (totalTasks%100 == 0)
                {
                    Console.WriteLine("Running: {0}, queued: {1}, Total: {2}", currentRunningProcessCount,
                                      CurrentThreadQueuedTasks.Count, totalTasks);
                }
            }
            task.Continue();
            return true;
        }

        /// <summary>
        /// This must run under the lock!
        /// </summary>
        private bool? TryGetTaskToRun(ref AbstractTask task)
        {
            //this approach gives us an O(1) _removal_ cost from the list
            LinkedListNode<SchedulePair> node = CurrentThreadQueuedTasks.First;
            while (node != null)
            {
                SchedulePair pair = node.Value;
                if (pair.Condition())
                {
                    task = pair.Task;
                    CurrentThreadQueuedTasks.Remove(node);
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
                    CurrentThreadQueuedTasks.Remove(prev);
                    CurrentThreadQueuedTasks.AddLast(prev);
                }
            }
            if (task == null) // no tasks to run in the curren threa, time to steal some work...
            {
                StealWorkFromAnotherThread();
                //nothing runs, and there are no tasks that we _can_ run
                // we are either deadlocked or waiting for an external resource
                // we will let the idle task decide what to do.
                if (currentRunningProcessCount == 0)
                {
                    Status = ScheduleStatus.Idle;
                    task = idleTask;
                    return true;
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