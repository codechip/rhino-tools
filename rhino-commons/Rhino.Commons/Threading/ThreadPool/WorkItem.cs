using System.Threading;

namespace Rhino.Commons
{
    public sealed class WorkItem
    {
        private WaitCallback _callback;
        private object _state;
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
    }
}