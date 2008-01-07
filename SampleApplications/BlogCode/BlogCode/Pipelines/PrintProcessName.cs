namespace pipelines.Pipelines
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class PrintProcessName : IOperation<Process>
    {
        public IEnumerable<Process> Execute(IEnumerable<Process> input)
        {
            foreach (Process process in input)
            {
                System.Console.WriteLine(process.ProcessName);
            }
            yield break;
        }
    }
}