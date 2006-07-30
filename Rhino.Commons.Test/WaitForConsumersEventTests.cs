using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class WaitForConsumersEventTests
    {
        [Test]
        public void WaitForEventWithTimeout()
        {
            WaitForConsumersEvent wait = new WaitForConsumersEvent(5);
            bool result = wait.WaitOne(TimeSpan.FromMilliseconds(5));
            Assert.IsFalse(result);
        }

        [Test]
        public void WaitForEventToRun()
        {
            int count = 5000;
            WaitForConsumersEvent wait = new WaitForConsumersEvent(count);
            bool[] done = new bool[count];
            for (int i = 0; i < count; i++)
            {
                int j = i;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    done[j] = true;
                    wait.Set();
                });
            }
            bool result = wait.WaitOne();
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
            new WaitForConsumersEvent(-2).WaitOne();
        }

        [Test]
        public void WaitForConsumersDoesNotWaitIfConsumersAreZero()
        {
            bool result = new WaitForConsumersEvent(0).WaitOne();
            Assert.IsTrue(result);
        }

        [Test]
        public void ResetWaitForConsumers()
        {
            WaitForConsumersEvent wait = new WaitForConsumersEvent(0);
            bool result = wait.WaitOne();
            Assert.IsTrue(result);
            wait.Reset(5);
            result = wait.WaitOne(TimeSpan.FromMilliseconds(5));
            Assert.IsFalse(result);
            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    wait.Set();
                });
            }
            result = wait.WaitOne();
            Assert.IsTrue(result);

        }
    }
}
