namespace pipelines.Pipelines
{
    using System.Diagnostics;

    public class TrivialProcessesPipeline : SingleThreadedPipeline<Process>
    {
        public TrivialProcessesPipeline()
        {
            Register(new GetAllProcesses());
            Register(new LimitByWorkingSetSize());
            Register(new PrintProcessName());
        }
    }
}