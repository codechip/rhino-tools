#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
