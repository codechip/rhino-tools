namespace pipelines.Pipelines
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class GetAllProcesses : IOperation<Process>
    {
        public IEnumerable<Process> Execute(IEnumerable<Process> input)
        {
            return Process.GetProcesses();
        }
    }
}