using System;
using System.Collections.Generic;
using System.Threading;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Impl
{
    public class InMemorySagaPersister<TSaga> : ISagaPersister<TSaga> 
        where TSaga : class, ISaga
    {
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly Dictionary<Guid, TSaga> dictionary = new Dictionary<Guid, TSaga>();

        public TSaga Get(Guid id)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                TSaga value;
                if (dictionary.TryGetValue(id, out value))
                    return value;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
            return null;
        }

        public void Save(TSaga saga)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                dictionary[saga.Id] = saga;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void Complete(TSaga saga)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                dictionary.Remove(saga.Id);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }
    }
}