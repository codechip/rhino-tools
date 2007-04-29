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

namespace NHibernate.Generics
{
	[Serializable]
    public class EntityDictionary<TKey, TValue> : 
        AbstractEntityCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        System.Collections.IDictionary inner = new System.Collections.Hashtable();

        public EntityDictionary() : base(null, null) 
        {

        }
                           
        public EntityDictionary(Action<KeyValuePair<TKey, TValue>> add, Action<KeyValuePair<TKey, TValue>> remove, InitializeOnLazy lazyBehavior) :
            base(add, remove, lazyBehavior)
        {
        }



        public EntityDictionary(Action<KeyValuePair<TKey, TValue>> add, Action<KeyValuePair<TKey, TValue>> remove) : 
            base(add, remove)
        {
        }

        protected override System.Collections.ICollection Collection
        {
            get { return inner; }
        }

        protected override bool DoAdd(KeyValuePair<TKey, TValue> item)
        {
            inner.Add(item.Key, item.Value);
            return true;
        }

        protected override void DoClear()
        {
            inner.Clear();
        }

        public override bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return inner.Contains(new System.Collections.DictionaryEntry(item.Key, item.Value));
        }

        protected override bool DoRemove(KeyValuePair<TKey, TValue> item)
        {
            inner.Remove(item.Key);
            return true;
        }

        protected override object InnerValue
        {
            get
            {
                return inner;
            }
            set
            {
                inner = (System.Collections.IDictionary)value;
            }
        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            inner.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return inner.Contains(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                TKey[] keys = new TKey[inner.Keys.Count];
                inner.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public bool Remove(TKey key)
        {
            inner.Remove(key);
            return true;
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (inner.Contains(key))
            {
                value = (TValue)inner[key];
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get 
            {
                TValue[] vals = new TValue[inner.Values.Count];
                inner.Values.CopyTo(vals, 0);
                return vals;
            }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                if (inner.Contains(key) == false)
                    throw new KeyNotFoundException();
                return (TValue)inner[key];
            }
            set
            {
                inner[key] = value;
            }
        }

        #endregion

        public override IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
        {
            foreach (System.Collections.DictionaryEntry item in Collection)
            {
                yield return new KeyValuePair<TKey,TValue>((TKey)item.Key, (TValue)item.Value);
            }
        }
    }
}
