using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
    public class DisposableAction<T> : IDisposable
    {
        Proc<T> _action;
        T _val;

        public DisposableAction(Proc<T> action, T val)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
            _val = val;
        }

        public void Dispose()
        {
            _action(_val);
        }
    }

    public class DisposableAction : IDisposable
    {
        Proc _action;

        public DisposableAction(Proc action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
