using System.Threading;
using MbUnit.Framework;
using Rhino.Commons;

[TestFixture]
public class WithPerformanceCounterTests
{
	[Test]
	public void CallsDelegateAndReturnNonZeroTiming()
	{
		bool called = false;
		double ticks = With.PerformanceCounter(delegate
		{
			called = true;
			Thread.Sleep(40);
		});
		//The only reliable way of testing this that I can think of. 
		Assert.IsTrue( ticks > 0);
		Assert.IsTrue(called);
	}
}