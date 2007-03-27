using System.Threading;

namespace Rhino.Commons
{
    public sealed class WorkItem
    {
        private WaitCallback _callback;
        private object _state;
        private bool cancelledRun;
        private ExecutionContext _ctx;

        internal WorkItem(WaitCallback wc, object state, ExecutionContext ctx)
        {
            _callback = wc;
            _state = state;
            _ctx = ctx;
        }

        internal WaitCallback Callback
        {
            get { return _callback; }
        }

        public object State
        {
            get { return _state; }
        }

        public ExecutionContext Context
        {
            get { return _ctx; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WorkItem"/> is canceled.
        /// This is valid if and only if the work item has not started running yet.
        /// If the WorkItem has started running, it is ignored
        /// </summary>
        /// <value><c>true</c> if cancelledRun; otherwise, <c>false</c>.</value>
        public bool Cancelled
        {
            get { return cancelledRun; }
            set { cancelledRun = value; }
        }
    }
}