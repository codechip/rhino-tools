namespace pipelines
{
    using System.Collections.Generic;

    public abstract class AbstractTask
    {
        private Scheduler scheduler;
        private readonly Future completed = new Future();
        private IEnumerator<Condition> state;
        private object result = null;

        public void SetResult(object value)
        {
            result = value;
        }

        public Future Completed
        {
            get { return completed; }
        }

        protected Future Spawn(AbstractTask task)
        {
            return scheduler.Schedule(task);
        }

        protected internal void Initialize(Scheduler aScheduler)
        {
            this.scheduler = aScheduler;
            state = Execute().GetEnumerator();
        }

        public void Continue()
        {
            bool hasNext = state.MoveNext();
            if (hasNext)
            {
                scheduler.Schedule(this, state.Current);
            }
            else
            {
                //It is important that we first set the task to complete
                // and then mark it complete in the schedule.
                // This is because setting it to complete in the scheduler will wake
                // waiting threads, and we want to mark the task as complete beforehand
                completed.SetValue(result);

                //we want to avoid stack overflow if we keep reusing the same stack
                // only the first level task will reuse a thread
                if (SchedulingOptions.NestedStackCount == 0)
                {
                    SchedulingOptions.NestedStackCount = 1;
                    while (true)
                    {
                        if (scheduler.TrySchedule(ExecuteTaskInSameThread, ThreadingOptions.ReuseCurrentThread) == false)
                            break;
                    }
                    SchedulingOptions.NestedStackCount = 0;
                }
                scheduler.Completed(this);
            }
        }

        public void ExecuteTaskInSameThread(AbstractTask task)
        {
            task.Continue();
        }

        public WaitForFuture Done(Future future)
        {
            return new WaitForFuture(future);
        }

        protected abstract IEnumerable<Condition> Execute();
    }
}