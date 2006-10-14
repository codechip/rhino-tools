using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NHibernate.Generics;

namespace NHibernate.Generics
{
    [Serializable]
    public class EntityRef<T> : IWrapper where T : class
    {
        private Action<T> _change, _clear;
        //Use to sync stuff
        private bool _inAssignmentOperation;
        private bool _callActions = true;
        private T _value;

        public IDisposable AllowModifications
        {
            get
            {
                _inAssignmentOperation = false;
                _callActions = false;
                return new DisposableAction(delegate { _inAssignmentOperation = true; _callActions = true; });
            }
        }

        public T Value
        {
            get { return _value; }
            set
            {
                //Avoid unnececary operations when there is no change
                //in value
                if (_value == value)
                    return;
                //Avoid double assignment
                if (_inAssignmentOperation)
                    return;
                _inAssignmentOperation = true;
                try
                {
                    if (_value != null)
                    {
                        if (NHibernateUtil.IsInitialized(_value))
                        {
                            // Only change the EntitySet on the one-side if this object is
                            // already initalized (no longer a proxy), otherwise the following 
                            // call will resolve the proxy and load the object, effectively disabling
                            // lazy loading
                            this.CallClear(_value);
                        }
                    }

                    _value = value;

                    if (_value != null)
                        if (NHibernateUtil.IsInitialized(_value))
                        {
                            // Only change the EntitySet on the one-side if this object is
                            // already initalized (no longer a proxy), otherwise the following 
                            // call will resolve the proxy and load the object, effectively disabling
                            // lazy loading
                            this.CallChange(_value);
                        }

                }
                finally
                {
                    _inAssignmentOperation = false;
                }
            }
        }

        public EntityRef()
            : this(null, null)
        { }

        public EntityRef(T initialValue)
            : this(initialValue, null, null)
        { }

        public EntityRef(Action<T> change, Action<T> clear)
        {
            this._inAssignmentOperation = false;
            this._change = change;
            this._clear = clear;
        }

        public EntityRef(T initalValue, Action<T> change, Action<T> clear)
            : this(change, clear)
        {
            this._value = initalValue;
        }

        #region IWrapper Members

        object IWrapper.Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = (T)value;
            }
        }

    	#endregion

        private void CallClear(T item)
        {
            if (_callActions && _clear != null)
                _clear(item);
        }

        private void CallChange(T item)
        {
            if (_callActions && _change != null)
                _change(item);
        }
    }
}
