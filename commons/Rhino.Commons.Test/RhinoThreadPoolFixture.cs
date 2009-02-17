namespace Rhino.Commons.Test
{
	using System.Collections.Generic;
	using System.Threading;
	using MbUnit.Framework;

	[TestFixture]
	public class RhinoThreadPoolFixture
	{
		private readonly List<string> output = new List<string>();

		private void Do_sleep(object state)
		{
			int index = (int)state;

			Add("index #" + index);

			Thread.Sleep(100);
		}

		public void Add(string msg)
		{
			lock (output)
			{
				output.Add(msg);
			}
		}

		[Test]
		public void WillWaitForAllThreadsToComplete()
		{
			List<WorkItem> items = new List<WorkItem>();

			for (int i = 0; i < 5; i++)
			{
				items.Add(RhinoThreadPool.QueueUserWorkItem(this.Do_sleep, i));
			}

			RhinoThreadPool.WaitForAllThreadsToFinish();

			Add("Finished...");

			lock (output)
			{
				Assert.AreEqual(6, output.Count);
				Assert.AreEqual("Finished...", output[5]);
			}
		}
	}
}
