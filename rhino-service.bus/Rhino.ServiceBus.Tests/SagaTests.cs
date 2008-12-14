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
                Component.For(typeof (ISagaPersister<>))
                    .ImplementedBy(typeof (InMemorySagaPersister<>)),
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

                Assert.Equal(1, processor.Messages.Count);
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

                bus.Send(bus.Endpoint, new AddLineItemMessage {CorrelationId = sagaId});

                wait.WaitOne();
                var persister = container.Resolve<ISagaPersister<OrderProcessor>>();
                OrderProcessor processor = null;
                while (processor == null)
                {
                    Thread.Sleep(500);
                    processor = persister.Get(sagaId);
                }

                Assert.Equal(2, processor.Messages.Count);
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

                bus.Send(bus.Endpoint, new SubmitOrderMessage() { CorrelationId = sagaId });

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

        public class OrderProcessor : InitiatedBy<NewOrderMessage>,
                                      Orchestrates<AddLineItemMessage>,
                                      Orchestrates<SubmitOrderMessage>
        {
            public List<object> Messages = new List<object>();

            #region InitiatedBy<NewOrderMessage> Members

            public void Consume(NewOrderMessage pong)
            {
                Messages.Add(pong);
                wait.Set();
            }

            public Guid Id { get; set; }
            public bool IsCompleted { get; set; }

            #endregion

            #region Orchestrates<AddLineItemMessage> Members

            public void Consume(AddLineItemMessage pong)
            {
                Messages.Add(pong);
                sagaId = Id;
                wait.Set();
            }

            #endregion

            public void Consume(SubmitOrderMessage message)
            {
                IsCompleted = true;
                wait.Set();
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