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
using System.Threading;

namespace Rhino.Commons
{
	/// <summary>
	/// Allows a master thread to wait for a set of
	/// subservient threads to complete work.
	/// </summary>
    public class CountdownLatch : IDisposable
    {
        int numberOfConsumers;
        ManualResetEvent doneWaitingEvent;

        public CountdownLatch(int numberOfConsumers)
        {
            bool initialState = SetConsumers(numberOfConsumers);
            doneWaitingEvent = new ManualResetEvent(initialState);
        }

        public bool WaitOne()
        {
            return doneWaitingEvent.WaitOne();
        }

        public bool WaitOne(TimeSpan timeout)
        {
            return doneWaitingEvent.WaitOne(timeout, false);
        }

        public int Set()
        {
            int val = Interlocked.Decrement(ref numberOfConsumers);
            if (val <= 0)
                doneWaitingEvent.Set();
        	return val;
        }

        public void Reset(int numberOfConsumers)
        {
            if (!SetConsumers(numberOfConsumers))
                doneWaitingEvent.Reset();
        }

        public void Dispose()
        {
            doneWaitingEvent.Close();
        }

        private bool SetConsumers(int numberOfConsumers)
        {
            if (numberOfConsumers < 0)
                throw new ArgumentOutOfRangeException("numberOfConsumers", numberOfConsumers, "Should be zero or greater");
            this.numberOfConsumers = numberOfConsumers;
            return numberOfConsumers == 0;
        }
    }
}
