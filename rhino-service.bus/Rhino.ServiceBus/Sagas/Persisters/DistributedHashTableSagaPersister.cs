using System;
using System.IO;
using Castle.MicroKernel;
using Rhino.DHT;
using Rhino.ServiceBus.Internal;
using System.Linq;
using Rhino.ServiceBus.Messages;

namespace Rhino.ServiceBus.Sagas.Persisters
{
    public class DistributedHashTableSagaPersister<TSaga, TState> : ISagaPersister<TSaga>
            where TSaga : class, ISaga<TState>
            where TState : IVersionedSagaState
    {
        private readonly IDistributedHashTable distributedHashTable;
        private readonly ISagaStateMerger<TState> stateMerger;
        private readonly IMessageSerializer messageSerializer;
        private readonly IKernel kernel;
        private readonly IServiceBus bus;
        private readonly IReflection reflection;

        public DistributedHashTableSagaPersister(IDistributedHashTable distributedHashTable, ISagaStateMerger<TState> stateMerger, IMessageSerializer messageSerializer, IKernel kernel, IReflection reflection, IServiceBus bus)
        {
            this.distributedHashTable = distributedHashTable;
            this.bus = bus;
            this.stateMerger = stateMerger;
            this.messageSerializer = messageSerializer;
            this.kernel = kernel;
            this.reflection = reflection;
        }

        public TSaga Get(Guid id)
        {
            var values = distributedHashTable.Get(new[]
            {
                new GetValue {Key = id.ToString()},
            }).First();
            if (values.Length == 0)
                return null;
            TState state;
            if (values.Length != 1)
            {
                var states = new TState[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    var value = values[i];
                    using (var ms = new MemoryStream(value.Data))
                    {
                        object[] msgs = messageSerializer.Deserialize(ms);
                        states[i] = (TState)msgs[0];
                        states[i].ParentVersions = value.ParentVersions;
                        states[i].Version = value.Version;
                    }
                }
                state = stateMerger.Merge(states);
                state.ParentVersions = values
                    .Select(x => x.Version)
                    .ToArray();
            }
            else
            {
                using (var ms = new MemoryStream(values[0].Data))
                {
                    object[] msgs = messageSerializer.Deserialize(ms);
                    state = (TState)msgs[0];
                    state.ParentVersions = new[] { values[0].Version };
                }
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
                        ParentVersions = state.ParentVersions
                    },
                });
                if(putResults[0].ConflictExists)
                {
                    bus.Send(bus.Endpoint, new MergeSagaState
                    {
                        CorrelationId = saga.Id
                    });
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
                bus.Send(bus.Endpoint, new MergeSagaState
                {
                    CorrelationId = saga.Id
                });
            }
        }
    }
}