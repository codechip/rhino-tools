using System;
using System.IO;
using System.Linq;
using Castle.MicroKernel;
using Rhino.DHT;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;

namespace Rhino.ServiceBus.Sagas.Persisters
{
    public class OptimisticDistributedHashTableSagaPersister<TSaga, TState> : ISagaPersister<TSaga>
        where TSaga : class, ISaga<TState>
        where TState : IVersionedSagaState
    {
        private readonly IDistributedHashTable distributedHashTable;
        private readonly IReflection reflection;
        private readonly IMessageSerializer messageSerializer;
        private readonly IKernel kernel;

        public OptimisticDistributedHashTableSagaPersister(IDistributedHashTable distributedHashTable, IReflection reflection, IMessageSerializer messageSerializer, IKernel kernel)
        {
            this.distributedHashTable = distributedHashTable;
            this.reflection = reflection;
            this.messageSerializer = messageSerializer;
            this.kernel = kernel;
        }

        public TSaga Get(Guid id)
        {
            var values = distributedHashTable.Get(new[]
            {
                new GetValue {Key = id.ToString()},
            }).First();

            if(values.Length==0)
                return null;

            var value = values[0];

            TState state;
            using (var ms = new MemoryStream(value.Data))
            {
                var msgs = messageSerializer.Deserialize(ms);
                state = (TState)msgs[0];
                state.Version = value.Version;
                state.ParentVersions = value.ParentVersions;
            }
            var saga = kernel.Resolve<TSaga>();
            saga.Id = id;
            reflection.Set(saga, "State", type => state);
            return saga;
        }

        public void Save(TSaga saga)
        {
            using (var message = new MemoryStream())
            {
                var state = (TState)reflection.Get(saga, "State");
                messageSerializer.Serialize(new object[] { state }, message);
                var putResults = distributedHashTable.Put(new[]
                {
                    new AddValue
                    {
                        Bytes = message.ToArray(),
                        Key = saga.Id.ToString(),
                        OptimisticConcurrency = true,
                        ParentVersions = (state.Version != 0 ? new[] { state.Version } : new int[0])
                    },
                });
                if (putResults[0].ConflictExists)
                {
                    throw new OptimisticConcurrencyException("Saga state is not the latest: " + saga.Id);
                }
            }
        }

        public void Complete(TSaga saga)
        {
            var state = (TState) reflection.Get(saga, "State");
            var removed = distributedHashTable.Remove(new[]
            {
                new RemoveValue
                {
                    Key = saga.Id.ToString(),
                    ParentVersions = state.ParentVersions
                },
            });
            if (removed[0] == false)
            {
                throw new OptimisticConcurrencyException("Saga state is not the latest: " + saga.Id);
            }
        }
    }
}