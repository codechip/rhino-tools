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
using System.Collections;

namespace NHibernate.Generics
{
    [Serializable]
    public class EntityList<T> : AbstractEntityCollection<T>, IList<T> where T : class
    {
        System.Collections.IList _list;
        OnDuplicate allowDuplicates;

        #region Constructors

        public EntityList()
            : this(null, null)
        {

        }

        public EntityList(OnDuplicate allowDuplicates)
            : this(null, null)
        {
            this.allowDuplicates = allowDuplicates;
        }


        public EntityList(IList list)
            : this(list, null, null)
        {
        }

        public EntityList(IList list, InitializeOnLazy lazyBehavior, OnDuplicate allowDuplicates)
            : this(list, null, null, lazyBehavior)
        {
			this.allowDuplicates = allowDuplicates;
        }


        public EntityList(Action<T> add, Action<T> remove, InitializeOnLazy lazyBehavior, OnDuplicate allowDuplicates)
            : base(add, remove, lazyBehavior)
        {
			this.allowDuplicates = allowDuplicates;
            SetInitialList(new ArrayList());
        }

        public EntityList(Action<T> add, Action<T> remove)
            : this(new ArrayList(), add, remove)
        { }

        public EntityList(Action<T> add, Action<T> remove, OnDuplicate allowDuplicates)
            : this(new ArrayList(), add, remove)
        {
			this.allowDuplicates = allowDuplicates;
        }


        public EntityList(IList list, Action<T> add, Action<T> remove, InitializeOnLazy lazyBehavior)
            : base(add, remove, lazyBehavior)
        {
            SetInitialList(list);
        }
        public EntityList(IList list, Action<T> add, Action<T> remove)
            : base(add, remove)
        {
            SetInitialList(list);
        }

        #endregion

        private void SetInitialList(IList list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            this._list = list;

        }


        protected override System.Collections.ICollection Collection
        {
            get { return _list; }
        }

        public override bool Contains(T item)
        {
            return _list.Contains(item);
        }

        protected override bool DoAdd(T item)
        {
            if (allowDuplicates == OnDuplicate.DoNotAdd &&
                _list.Contains(item))
            {
                return false;
            }
            else
            {
                _list.Add(item);
                return true;
            }
        }

        protected override void DoClear()
        {
            _list.Clear();
        }

        protected override bool DoRemove(T item)
        {
            _list.Remove(item);
            return true;
        }

        protected override object InnerValue
        {
            get
            {
                return _list;
            }
            set
            {
                _list = (System.Collections.IList)value;
            }
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }



        #region IList<T> Members


        public void Insert(int index, T item)
        {
            SafeAccess(item, delegate(T t) { _list.Insert(index, t); return true; }, CallAdd);
        }

        public void RemoveAt(int index)
        {
            SafeAccess((T)_list[index], delegate(T t) { _list.RemoveAt(index); return true; }, CallRemove);
        }

        public T this[int index]
        {
            get
            {
                return (T)_list[index];
            }
            set
            {
                SafeAccess(delegate
                {
                    CallRemove((T)_list[index]);
                    _list[index] = value;
                    CallAdd(value);
                });
            }
        }

        #endregion
    }
}
