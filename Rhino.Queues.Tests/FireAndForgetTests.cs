using System;
using System.Threading;
using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class FireAndForgetTests
	{
		
		[Test]
		public void Can_register_action_in_fire_and_forget_and_it_will_be_executed_later()
		{
			bool wasCalled = false;
			FireAndForget.GetWaitTimespan = TimeSpan.FromMilliseconds;

			FireAndForget.Exceute(() => wasCalled = true);
			Thread.Sleep(50);
			Assert.IsTrue(wasCalled);
		}

		[Test]
		public void Will_retry_failing_action_several_times()
		{
			FireAndForget.GetWaitTimespan = TimeSpan.FromMilliseconds;
			int count = 0;
			FireAndForget.Exceute(() =>
			{
				if(Interlocked.Increment(ref count) < 3)
					throw new InvalidOperationException();
			});
			Thread.Sleep(50);
			Assert.AreEqual(3, count);
		}

		[Test]
		public void After_five_tries_will_raise_corrupt_state_exception()
		{
			bool corruptStateRaised=false;
			FireAndForget.GetWaitTimespan = TimeSpan.FromMilliseconds;
			FireAndForget.QueueAndMessageStateIsLikelyCorrupted+=delegate
			{
				corruptStateRaised = true;
			};
			int count = 0;
			FireAndForget.Exceute(() =>
			{
				count++;
				throw new InvalidOperationException();
			});
			Thread.Sleep(500);
			Assert.AreEqual(5, count);
			Assert.IsTrue(corruptStateRaised);
		}
	}
}