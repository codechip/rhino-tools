namespace pipelines.Pipelines
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class LimitByWorkingSetSize : IOperation<Process>
    {
        public IEnumerable<Process> Execute(IEnumerable<Process> input)
        {
            int maxSizeBytes = 50 * 1024 * 1024;
            foreach (Process process in input)
            {
                if (process.WorkingSet64 > maxSizeBytes)
                    yield return process;
            }
        }
    }
}