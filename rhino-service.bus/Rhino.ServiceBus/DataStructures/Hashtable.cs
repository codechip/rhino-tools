using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Rhino.ServiceBus.DataStructures
{

    public delegate void AddAction<TKey,TVal>(TKey key, TVal value);

    public delegate void RemoveAction<TKey>(TKey key);

    public delegate bool TryGetAction<TKey, TVal>(TKey key, out TVal value);

    public delegate void ReadAction<TKey, TVal>(TryGetAction<TKey, TVal> tryGet);

    public delegate void WriteAction<TKey, TVal>(AddAction<TKey, TVal> add, RemoveAction<TKey> remove, TryGetAction<TKey, TVal> tryGet);


    public class Hashtable<TKey,TVal> : IEnumerable<KeyValuePair<TKey,TVal>>
    {
        private readonly ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey,TVal> dictionary = new Dictionary<TKey, TVal>();

        public void Write(WriteAction<TKey, TVal> action)
        {
            readerWriterLockSlim.EnterWriteLock();
            try
            {
                action((key, val) => dictionary[key] = val,key => dictionary.Remove(key), TryGetInternal);
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        private bool TryGetInternal(TKey key, out TVal value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Read(ReadAction<TKey, TVal> read)
       {
           readerWriterLockSlim.EnterReadLock();
           try
           {
               read(TryGetInternal);
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