namespace pipelines
{
    using System.Collections.Generic;
    using System.Threading;

    public class Scheduler
    {
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
        private readonly List<SchedulePair> tasks = new List<SchedulePair>();
        private readonly List<AbstractTask> runningTasks = new List<AbstractTask>();

        public Future Schedule(AbstractTask task)
        {
            task.Initialize(this);
            lock (syncLock)
            {
                tasks.Add(new SchedulePair(task, delegate { return true; }));
                runningTasks.Remove(task);
                Monitor.Pulse(syncLock);
            }
            return task.Completed;
        }

        public void Schedule(AbstractTask task, Condition condition)
        {
            lock (syncLock)
            {
                tasks.Add(new SchedulePair(task, condition));
                runningTasks.Remove(task);
                Monitor.Pulse(syncLock);
            }
        }

        public void Execute()
        {
            while (true)
            {
                AbstractTask task = null;
                lock (syncLock)
                {
                    if (tasks.Count == 0 && runningTasks.Count == 0)
                        return;
                    SchedulePair pairToRemove = null;
                    foreach (SchedulePair pair in tasks)
                    {
                        if (pair.Condition())
                        {
                            task = pair.Task;
                            pairToRemove = pair;
                            break;
                        }
                    }
                    if(pairToRemove!=null)
                    {
                        tasks.Remove(pairToRemove);
                    }
                    if (task == null)// no tasks to run
                    {
                        if (runningTasks.Count == 0)//nothing runs, we are probably deadlocked
                            throw new DeadLockException();
                        Monitor.Wait(syncLock);// wait for a change
                        continue;
                    }
                    runningTasks.Add(task);

                }
                ThreadPool.QueueUserWorkItem(delegate(object state)
                {
                    AbstractTask theTask = (AbstractTask)state;
                    theTask.Continue();
                }, task);
            }
        }

        public void Completed(AbstractTask task)
        {
            lock (syncLock)
            {
                runningTasks.Remove(task);
                Monitor.Pulse(syncLock);
            }
        }
    }
}