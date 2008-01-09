namespace pipelines
{
    using System;
    using System.Collections.Generic;

    public class WaitFiveSeconds : AbstractTask
    {
        protected override IEnumerable<Condition> Execute()
        {
            Console.WriteLine("Started {0}", DateTime.Now);
            DateTime in5Seconds = DateTime.Now.AddSeconds(5);
            yield return delegate { return DateTime.Now >= in5Seconds; };
            Console.WriteLine("Finished: {0}", DateTime.Now);
        }
    }
}