using System;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Impl
{
    public class InMemorySagaPersister<TSaga> : ISagaPersister<TSaga> 
        where TSaga : class, IAccessibleSaga
    {
        private readonly Hashtable<Guid, TSaga> dictionary = new Hashtable<Guid, TSaga>();

        public TSaga Get(Guid id)
        {
            TSaga val = null;
            dictionary.Read(reader => reader.TryGetValue(id, out val));
            return val;
        }

        public void Save(TSaga saga)
        {
            dictionary.Write(writer => writer.Add(saga.Id, saga));
        }

        public void Complete(TSaga saga)
        {
            dictionary.Write(writer => writer.Remove(saga.Id));
        }
    }
}