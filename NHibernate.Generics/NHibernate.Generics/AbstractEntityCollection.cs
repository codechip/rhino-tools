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
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;

namespace NHibernate.Generics
{
    [Serializable]
    public abstract class AbstractEntityCollection<T> : ICollection<T>, IWrapper
    {
        public delegate void Proc();

        private readonly InitializeOnLazy _lazyBehavior = InitializeOnLazy.Never;
        private readonly Action<T> _add;
        private readonly Action<T> _remove;
        
        private bool _isInModificationOperation = false;
        private bool _callActions = true;
        private object syncRoot = new object();

        protected AbstractEntityCollection(Action<T> add, Action<T> remove, InitializeOnLazy lazyBehavior)
        {
            this._add = add;
            this._remove = remove;
            this._lazyBehavior = lazyBehavior;   
        }

        protected AbstractEntityCollection(Action<T> add, Action<T> remove)
        {
            this._add = add;
            this._remove = remove;
        }

        protected abstract System.Collections.ICollection Collection
        {
            get;
        }

        public IDisposable AllowModifications
        {
            get
            {
                _isInModificationOperation = false;
                _callActions = false;
                return new DisposableAction(delegate { _isInModificationOperation = true; _callActions = true; }); 
            }
        }

        public bool IsInitialized
        {
            get
            {
                return NHibernateUtil.IsInitialized(Collection);
            }
        }

        public void Load()
        {
            NHibernateUtil.Initialize(Collection);
        }

        public void Add(T item)
        {
            SafeAccess(item, DoAdd, CallAdd);
        }

        /// <summary>
        /// Run an action safe from re-enterant issues (NOT thread safe).
        /// This overload is for working on a single item within the collection.
        /// </summary>
        protected bool SafeAccess(T item, Predicate<T> act, Action<T> notify)
        {
            //Avoids double action
            if (_isInModificationOperation)
                return false;
            _isInModificationOperation = true;
            try
            {
                bool result = false;
                //Only act if it's initialized, since the 
                //conection is assumed to be maintained on the other end (EntityRef).
                //However, if the _lazyBehavior is set to Always, will always initialize.
                if (ShouldInitialize)
                {
                    result = act(item);
                }
                notify(item);
                return result;
            }
            finally
            {
                _isInModificationOperation = false;
            }
        }


        /// <summary>
        /// Run an action safe from re-enterant issues (NOT thread safe).
        /// This overload is for working on mutliply items in the collection.
        /// </summary>
        protected void SafeAccess(Proc action)
        {
            //Avoids double action
            if (_isInModificationOperation)
                return;
            _isInModificationOperation = true;
            try
            {
                action();
            }
            finally
            {
                _isInModificationOperation = false;
            }
        }

        protected abstract bool DoAdd(T item);

        private bool ShouldInitialize
        {
            get 
            {
                return
                    _lazyBehavior == InitializeOnLazy.Always ||
                    IsInitialized;
	            
            }
        }

        /// <summary>
        /// Clears all the items in the set, calling the remove delegate
        /// for each one of them.
        /// This method will always loads the collection if it is not loaded.
        /// </summary>
        public void Clear()
        {
            SafeAccess(RunClear);
        }

        private void RunClear()
        {
            T[] items = new T[Collection.Count];
            Collection.CopyTo(items, 0);
            foreach (object item in items)
            {
                CallRemove((T)item);
            }
            DoClear();
        }

        protected abstract void DoClear();

        public abstract bool Contains(T item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get 
            {
                return Collection.Count; 
            }
        }

        public bool IsReadOnly 
        {			
            get  { 	return false; }
        }

        public bool Remove(T item)
        {
            return SafeAccess(item, DoRemove, CallRemove);
        }

        protected abstract bool DoRemove(T item);

        public virtual IEnumerator<T> GetEnumerator()
        {
            foreach (T item in Collection)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        /// <summary>
        /// This should be used by the <c>GenericWrapperAccessor</c>
        /// </summary>
        object IWrapper.Value
        {
            get
            {
                return InnerValue;
            }
            set
            {
                InnerValue = value;
            }
        }

        protected abstract object InnerValue
        {
            get;
            set;
        }

        protected void CallRemove(T item)
        {
            if (_callActions && _remove != null)
                _remove(item);
        }

        protected void CallAdd(T item)
        {
            if (_callActions && _add != null)
                _add(item);
        }
    }
}