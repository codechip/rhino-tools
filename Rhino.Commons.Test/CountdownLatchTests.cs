using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.Threading;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class CountdownLatchTests
    {
        [Test]
        public void WaitForEventWithTimeout()
        {
            CountdownLatch countdown = new CountdownLatch(5);
            bool result = countdown.WaitOne(TimeSpan.FromMilliseconds(5));
            Assert.IsFalse(result);
        }

        [Test]
        public void WaitForEventToRun()
        {
            int count = 5000;
            CountdownLatch countdown = new CountdownLatch(count);
            bool[] done = new bool[count];
            for (int i = 0; i < count; i++)
            {
                int j = i;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    done[j] = true;
                    countdown.Set();
                });
            }
            bool result = countdown.WaitOne();
            Assert.IsTrue(result);
            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(done[i], "{0} was not set to true", i);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Should be zero or greater\r\nParameter name: numberOfConsumers\r\nActual value was -2.")]
        public void WaitForConumersThrowsIfGetsNegativeConsumers()
        {
            new CountdownLatch(-2).WaitOne();
        }

        [Test]
        public void WaitForConsumersDoesNotWaitIfConsumersAreZero()
        {
            bool result = new CountdownLatch(0).WaitOne();
            Assert.IsTrue(result);
        }

        [Test]
        public void ResetWaitForConsumers()
        {
            CountdownLatch countdown = new CountdownLatch(0);
            bool result = countdown.WaitOne();
            Assert.IsTrue(result);
            countdown.Reset(5);
            result = countdown.WaitOne(TimeSpan.FromMilliseconds(5));
            Assert.IsFalse(result);
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    countdown.Set();
                });
            }
            result = countdown.WaitOne();
            Assert.IsTrue(result);

        }
    }
}
