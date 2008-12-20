using System;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Impl
{
    public class InMemorySagaPersister<TSaga> : ISagaPersister<TSaga> 
        where TSaga : class, ISaga
    {
        private readonly Hashtable<Guid, TSaga> dictionary = new Hashtable<Guid, TSaga>();

        public TSaga Get(Guid id)
        {
            TSaga val = null;
            dictionary.Read(get => get(id, out val));
            return val;
        }

        public void Save(TSaga saga)
        {
            dictionary.Write((add,remove, tryGet) => add(saga.Id, saga));
        }

        public void Complete(TSaga saga)
        {
            dictionary.Write((add, remove, tryGet) => remove(saga.Id));
        }
    }
}