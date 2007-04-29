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
