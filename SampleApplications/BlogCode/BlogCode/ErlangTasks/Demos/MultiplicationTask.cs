namespace pipelines
{
    using System.Collections.Generic;
    using System.Threading;

    public class MultiplicationTask : AbstractTask
    {
        private readonly int value;

        public MultiplicationTask(int value)
        {
            this.value = value;
        }

        protected override IEnumerable<Condition> Execute()
        {
            Thread.Sleep(1000);
            SetResult(value * 2);
            yield break;
        }
    }
}