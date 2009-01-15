using System;
using System.Collections.Generic;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class SagaTests : MsmqBehaviorTests
    {
        private static Guid sagaId;
        private static ManualResetEvent wait;
        private readonly IWindsorContainer container;

        public SagaTests()
        {
            wait = new ManualResetEvent(false);
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
            container.Register(
                Component.For(typeof(ISagaPersister<>))
                    .ImplementedBy(typeof(InMemorySagaPersister<>)),
                Component.For<OrderProcessor>()
                );
        }

        [Fact]
        public void Can_create_saga_entity()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new NewOrderMessage());
                wait.WaitOne();

                var persister = container.Resolve<ISagaPersister<OrderProcessor>>();
                OrderProcessor processor = null;
                while (processor == null)
                {
                    Thread.Sleep(500);
                    processor = persister.Get(sagaId);
                }

                Assert.Equal(1, processor.State.Count);
            }
        }

        [Fact]
        public void When_creating_saga_entity_will_set_saga_id()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new NewOrderMessage());
                wait.WaitOne();

                var persister = container.Resolve<ISagaPersister<OrderProcessor>>();
                OrderProcessor processor = null;
                while (processor == null)
                {
                    Thread.Sleep(500);
                    processor = persister.Get(sagaId);
                }

                Assert.NotEqual(Guid.Empty, sagaId);
            }
        }

        [Fact]
        public void Can_send_several_messaged_to_same_instance_of_saga_entity()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new NewOrderMessage());
                wait.WaitOne();
                wait.Reset();
                
                var persister = container.Resolve<ISagaPersister<OrderProcessor>>();
            	OrderProcessor processor = null;

            	for (int i = 0; i < 1000; i++)
            	{
					processor = persister.Get(sagaId);
					if (processor!=null)
						break;
					Thread.Sleep(250);
            	}

                Assert.Equal(1, processor.State.Count);

                bus.Send(bus.Endpoint, new AddLineItemMessage { CorrelationId = sagaId });

                wait.WaitOne();
               
                Assert.Equal(2, processor.State.Count);
            }
        }

        [Fact]
        public void Completing_saga_will_get_it_out_of_the_in_memory_persister()
        {
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                bus.Start();

                bus.Send(bus.Endpoint, new NewOrderMessage());
                wait.WaitOne();
                wait.Reset();

                var persister = container.Resolve<ISagaPersister<OrderProcessor>>();
                OrderProcessor processor = null;
                while (processor == null)
                {
                    Thread.Sleep(500);
                    processor = persister.Get(sagaId);
                }

                bus.Send(bus.Endpoint, new SubmitOrderMessage { CorrelationId = sagaId });

                wait.WaitOne();

                while (processor != null)
                {
                    Thread.Sleep(500);
                    processor = persister.Get(sagaId);
                }

                Assert.Null(processor);
            }
        }

        #region Nested type: AddLineItemMessage

        public class AddLineItemMessage : ISagaMessage
        {
            #region ISagaMessage Members

            public Guid CorrelationId { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: NewOrderMessage

        public class NewOrderMessage
        {
        }

        #endregion

        #region Nested type: OrderProcessor

        public class OrderProcessor : 
                ISaga<List<object>>,
                InitiatedBy<NewOrderMessage>,
                Orchestrates<AddLineItemMessage>,
                Orchestrates<SubmitOrderMessage>
        {
            public OrderProcessor()
            {
                State = new List<object>();
            }
            #region InitiatedBy<NewOrderMessage> Members

            public void Consume(NewOrderMessage pong)
            {
                State.Add(pong);
                sagaId = Id;
                wait.Set();
            }

            public Guid Id { get; set; }
            public bool IsCompleted { get; set; }

            #endregion

            #region Orchestrates<AddLineItemMessage> Members

            public void Consume(AddLineItemMessage pong)
            {
                State.Add(pong);
                sagaId = Id;
                wait.Set();
            }

            #endregion

            public void Consume(SubmitOrderMessage message)
            {
                IsCompleted = true;
                wait.Set();
            }

            public List<object> State
            {
                get; set;
            }
        }

        #endregion

        #region Nested type: SubmitOrderMessage

        public class SubmitOrderMessage : ISagaMessage
        {
            #region ISagaMessage Members

            public Guid CorrelationId { get; set; }

            #endregion
        }

        #endregion
    }
}