using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.DHT;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;
using Xunit;
using System.Linq;

namespace Rhino.ServiceBus.Tests.Dht
{
    public class DistributedHashTableSagaPersisterTests : MsmqTestBase,
        OccasionalConsumerOf<DrinkReady>
    {
        private readonly IWindsorContainer container;

        public DistributedHashTableSagaPersisterTests()
        {
            File.Delete("cache.esent");
            BaristaSaga.WaitToCreateConflicts = new ManualResetEvent(true);
            BaristaSaga.FinishedConsumingMessage = new ManualResetEvent(false);
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", 
                new RhinoServiceBusFacility()
                    .UseDhtSagaPersister()
                );
            container.AddComponent<IDistributedHashTable, DistributedHashTable>();
            container.AddComponent<BaristaSaga>();
            container.AddComponent<ISagaStateMerger<BaristaState>, BaristaStateMerger>();
        }

        [Fact]
        public void Will_put_saga_state_in_dht()
        {
            var guid = Guid.NewGuid();
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new PrepareDrink
                {
                    CorrelationId = guid,
                    CustomerName = "ayende",
                    DrinkName = "Coffee"
                });

                BaristaSaga.FinishedConsumingMessage.WaitOne();
            }

            var distributedHashTable = container.Resolve<IDistributedHashTable>();
            Value[] values;

            do
            {
                Thread.Sleep(100);
                values = distributedHashTable.Get(new[]
                {
                    new GetValue
                    {
                        Key = guid.ToString()
                    },
                }).First();
            } while (values.Length==0);

            var messageSerializer = container.Resolve<IMessageSerializer>();
            var state = (BaristaState)messageSerializer.Deserialize(new MemoryStream(values[0].Data))[0];

            Assert.True(state.DrinkIsReady);
            Assert.False(state.GotPayment);
            Assert.Equal("Coffee", state.Drink);
        }

        [Fact]
        public void When_saga_complete_will_remove_from_dht()
        {
            var guid = Guid.NewGuid();

            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new PrepareDrink
                {
                    CorrelationId = guid,
                    CustomerName = "ayende",
                    DrinkName = "Coffee"
                });

                BaristaSaga.FinishedConsumingMessage.WaitOne();
                BaristaSaga.FinishedConsumingMessage.Reset();

                using(bus.AddInstanceSubscription(this))
                {
                    bus.Send(bus.Endpoint, new PaymentComplete
                    {
                        CorrelationId = guid,
                    });
                    BaristaSaga.FinishedConsumingMessage.WaitOne();
                }
            }

            var distributedHashTable = container.Resolve<IDistributedHashTable>();
           
            Value[] values;
            do
            {
                Thread.Sleep(100);
                values = distributedHashTable.Get(new[]
                {
                    new GetValue
                    {
                        Key = guid.ToString()
                    },
                }).First();
            } while (values.Length != 0);

            Assert.Equal(0, values.Length);
        }

        [Fact]
        public void When_dht_contains_conflicts_when_saga_is_completed_will_call_saga_again()
        {
            var guid = Guid.NewGuid();

            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new PrepareDrink
                {
                    CorrelationId = guid,
                    CustomerName = "ayende",
                    DrinkName = "Coffee"
                });

                BaristaSaga.FinishedConsumingMessage.WaitOne();
                BaristaSaga.FinishedConsumingMessage.Reset();

                BaristaSaga.WaitToCreateConflicts = new ManualResetEvent(false);

                using (bus.AddInstanceSubscription(this))
                {
                    bus.Send(bus.Endpoint, new PaymentComplete
                    {
                        CorrelationId = guid,
                    });
                    BaristaSaga.FinishedConsumingMessage.WaitOne();

                    BaristaSaga.FinishedConsumingMessage.Reset();
                    
                    var sagaPersister = container.Resolve<ISagaPersister<BaristaSaga>>();
                    var saga = new BaristaSaga(bus)
                    {
                        Id = guid,
                        State = {Drink = "foo"}
                    };
                    sagaPersister.Save(saga);

                    BaristaSaga.WaitToCreateConflicts.Set();
                }
                BaristaSaga.FinishedConsumingMessage.WaitOne();
            }


            Assert.Equal("foo", BaristaSaga.DrinkName);
        }

        public void Consume(DrinkReady message)
        {
            
        }

        public override void  Dispose()
        {
            base.Dispose();
            container.Dispose();
        }
    }
}