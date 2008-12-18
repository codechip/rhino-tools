using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Rhino.ServiceBus.DataStructures
{
    public class Hashtable<TKey,TVal> : IEnumerable<KeyValuePair<TKey,TVal>>
    {
        private readonly ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey,TVal> dictionary = new Dictionary<TKey, TVal>();

        public delegate void WriteAction(Action<TKey, TVal> add, Action<TKey> remove);

        public void Write(WriteAction action)
        {
            readerWriterLockSlim.EnterWriteLock();
            try
            {
                action((key, val) => dictionary[key] = val,key => dictionary.Remove(key));
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        public bool TryGet(TKey key, out TVal val)
        {
            readerWriterLockSlim.EnterReadLock();
            try
            {
                return dictionary.TryGetValue(key, out val);
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator()
        {
            readerWriterLockSlim.EnterReadLock();
            try
            {
                return dictionary
                    .ToList()
                    .GetEnumerator();
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}