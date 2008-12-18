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
            TSaga val;
            if(dictionary.TryGet(id, out val))
                return val;
            return null;
        }

        public void Save(TSaga saga)
        {
            dictionary.Write((add,remove) => add(saga.Id, saga));
        }

        public void Complete(TSaga saga)
        {
            dictionary.Write((add, remove) => remove(saga.Id));
        }
    }
}