using System;
using System.Collections;

namespace Rhino.ETL
{
	using Engine;

	public class TestPipeLineStage : PipeLineStage
	{
		public TestPipeLineStage(Pipeline pipeline, string incoming, IOutput output, string outgoing, int batchSize, IDictionary parameters) : base(pipeline, incoming, output, outgoing, batchSize, parameters)
		{
		}

		public TestOutput TestOutput
		{
			get { return (TestOutput) Output; }
		}

		public void OnProcess(string queueName, Action<Row> action)
		{
			TestOutput.OnProcess(queueName, action);
		}

		public void OnComplete(OutputCompleted cmd)
		{
			TestOutput.Completed += cmd;
		}
	}
}