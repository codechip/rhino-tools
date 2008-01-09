namespace pipelines
{
    using System;
    using System.Collections.Generic;

    public abstract class AbstractTask
    {
        private readonly Future completed = new Future();

        private ExecutionState executionState = ExecutionState.NotStarted;
        private object result = null;
        private Scheduler scheduler;
        private IEnumerator<Condition> state;

        public ExecutionState ExecutionState
        {
            get { return executionState; }
            set { executionState = value; }
        }

        public Future Completed
        {
            get { return completed; }
        }

        public void SetResult(object value)
        {
            result = value;
        }

        protected Future Spawn(AbstractTask task)
        {
            return scheduler.Schedule(task);
        }

        protected internal void Initialize(Scheduler aScheduler)
        {
            scheduler = aScheduler;
            state = Execute().GetEnumerator();
        }

        public void Continue()
        {
            ExecutionState = ExecutionState.Running;
            bool hasNext = ExecuteCurrentWork();
            if (hasNext)
            {
                scheduler.Reschedule(this, state.Current);
            }
            else
            {
                //It is important that we first set the task to complete
                // and then mark it complete in the schedule.
                // This is because setting it to complete in the scheduler will wake
                // waiting threads, and we want to mark the task as complete beforehand
                completed.SetValue(result);

                if (ExecutionState == ExecutionState.Running)
                    ExecutionState = ExecutionState.Completed;

                scheduler.Completed(this);
            }
        }

        private bool ExecuteCurrentWork()
        {
            try
            {
                return state.MoveNext();
            }
            catch (Exception e)
            {
                completed.SetError(e);
                ExecutionState = ExecutionState.Error;
                return false;
            }
        }

        public WaitForFuture Done(Future future)
        {
            return new WaitForFuture(future);
        }

        public WaitForFuture Done(params Future[] futures)
        {
            return Done((ICollection<Future>)futures);
        }

        public WaitForFuture Done(ICollection<Future> enumerable)
        {
            return new WaitForFuture(delegate
            {
                foreach (Future future in enumerable)
                {
                    if (future.HasValue == false)
                        return false;
                }
                return true;
            });
        }

        protected abstract IEnumerable<Condition> Execute();
    }
}