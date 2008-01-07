namespace pipelines.ErlangTasks.Demos
{
    using System.Collections.Generic;

    public class GeneratorTask : AbstractTask
    {
        private readonly int count;

        public GeneratorTask(int count)
        {
            this.count = count;
        }

        protected override IEnumerable<Condition> Execute()
        {
            for (int i = 0; i < count; i++)
            {
                Spawn(new GeneratorTask(count - 1));
            }
            yield break;
        }
    }
}