using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Generics
{
    public class DisposableAction<T> : IDisposable
    {
        Action<T> _action;
        T _val;

        public DisposableAction(Action<T> action, T val)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
            _val = val;
        }


        #region IDisposable Members

        public void Dispose()
        {
            _action(_val);
        }

        #endregion
    }

    public delegate void Action();

    public class DisposableAction : IDisposable
    {

        Action _action;

        public DisposableAction(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _action = action;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _action();
        }

        #endregion
    }
}
