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
