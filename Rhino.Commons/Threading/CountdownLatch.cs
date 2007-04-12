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

        public void Set()
        {
            int val = Interlocked.Decrement(ref numberOfConsumers);
            if (val == 0)
                doneWaitingEvent.Set();
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
