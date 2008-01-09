namespace pipelines
{
    using System.Collections.Generic;

    public class YieldOnIdleTask : AbstractTask
    {
        protected override IEnumerable<Condition> Execute()
        {
            while(true)
            {
                ThreadUtil.Yield();
                yield return delegate { return true; };
            }
        }
    }
}