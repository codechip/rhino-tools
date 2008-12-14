using System;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Internal
{
    public interface ISagaPersister<TSaga>
        where TSaga : class, ISaga
    {
        TSaga Get(Guid id);
        void Save(TSaga saga);
        void Complete(TSaga saga);
    }
}