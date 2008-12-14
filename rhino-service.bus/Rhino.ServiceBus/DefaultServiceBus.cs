using System;
using System.Collections;
using System.Collections.Generic;
using Castle.MicroKernel;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using System.Linq;
using Rhino.ServiceBus.Sagas;
using Rhino.ServiceBus.Util;

namespace Rhino.ServiceBus
{
    public class DefaultServiceBus : IStartableServiceBus
    {
        private readonly ITransport transport;
        private readonly ISubscriptionStorage subscriptionStorage;
        private readonly IReflection reflection;
        private readonly IKernel kernel;

        private readonly ILog logger = LogManager.GetLogger(typeof(DefaultServiceBus));

        public DefaultServiceBus(
            IKernel kernel,
            ITransport transport,
            ISubscriptionStorage subscriptionStorage,
            IReflection reflection
            )
        {
            this.transport = transport;
            this.subscriptionStorage = subscriptionStorage;
            this.reflection = reflection;
            this.kernel = kernel;
        }

        public void Publish(params object[] messages)
        {
            if (PublishInternal(messages) == false)
                throw new MessagePublicationException("There were no subscribers for (" +
                                                      messages.First() + ")"
                    );
        }

        public void Notify(params object[] messages)
        {
            PublishInternal(messages);
        }

        public void Reply(params object[] messages)
        {
            transport.Reply(messages);
        }

        public void Send(Uri endpoint, params object[] messages)
        {
            transport.Send(endpoint, messages);
        }

        public IServiceBus AddInstanceSubscription(IMessageConsumer consumer)
        {
            subscriptionStorage.AddInstanceSubscription(consumer);
            return this;
        }

        public Uri Endpoint
        {
            get { return transport.Endpoint; }
        }

        private bool PublishInternal(object[] messages)
        {
            bool sentMsg = false;
            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot publish an empty message batch");
            var msg = messages[0];
            var subscriptions = subscriptionStorage.GetSubscriptionsFor(msg.GetType());
            foreach (var subscription in subscriptions)
            {
                transport.Send(subscription, messages);
                sentMsg = true;
            }
            return sentMsg;
        }

        public void Dispose()
        {
            transport.Stop();
        }

        public void Start()
        {
            transport.MessageArrived += Transport_OnMessageArrived;
            transport.Start();
        }

        public void Transport_OnMessageArrived(CurrentMessageInformation msg)
        {
            object[] consumers = GatherConsumers(msg);

            if (consumers.Length == 0)
            {
                logger.ErrorFormat("Got message {0}, but had no consumers for it", msg);
                return;
            }
            try
            {
                foreach (var consumer in consumers)
                {
                    reflection.InvokeConsume(consumer, msg.Message);

                    var sagaEntity = consumer as ISaga;
                    if (sagaEntity == null)
                        continue;
                    PersistSagaInstance(sagaEntity);
                }
            }
            finally
            {
                foreach (var consumer in consumers)
                {
                    kernel.ReleaseComponent(consumer);
                }
            }
        }

        private void PersistSagaInstance(ISaga saga)
        {
            var persisterType = reflection.GetGenericTypeOf(typeof(ISagaPersister<>), saga);
            var persister = kernel.Resolve(persisterType);

            if (saga.IsCompleted)
                reflection.InvokeSagaPersisterComplete(persister, saga);
            else
                reflection.InvokeSagaPersisterSave(persister, saga);
        }

        private object[] GatherConsumers(CurrentMessageInformation msg)
        {
            var sagas = GetSagasFor(msg.Message as ISagaMessage);

            var instanceConsumers = subscriptionStorage
                .GetInstanceSubscriptions(msg.Message.GetType());

            var consumerType = reflection.GetGenericTypeOf(typeof(ConsumerOf<>), msg.Message);
            var consumers = (object[])kernel.ResolveAll(consumerType, new Hashtable());
            return instanceConsumers
                .Union(sagas)
                .Union(consumers)
                .ToArray();
        }

        private object[] GetSagasFor(ISagaMessage sagaMessage)
        {
            if (sagaMessage == null)
                return new object[0];

            var instances = new List<object>();
            var sagaInitiatedByThisMessage = reflection.GetGenericTypeOf(typeof(InitiatedBy<>), sagaMessage);

            var initiated = (object[])kernel.ResolveAll(sagaInitiatedByThisMessage, new Hashtable());

            foreach (ISaga saga in initiated)
            {
                saga.Id = GuidCombGenerator.Generate();
            }

            instances.AddRange(initiated);

            var messageType = reflection.GetGenericTypeOf(typeof(Orchestrates<>), sagaMessage);

            var handlers = kernel.GetAssignableHandlers(messageType);

            foreach (var sagaPersisterHandler in handlers)
            {
                var sagaPersisterType = reflection.GetGenericTypeOf(typeof (ISagaPersister<>),
                                                                    sagaPersisterHandler.ComponentModel.Implementation);

                var sagaPersister = kernel.Resolve(sagaPersisterType);
                try
                {
                    var sagaInstance = reflection.InvokeSagaPersisterGet(sagaPersister, sagaMessage.CorrelationId);
                    if (sagaInstance != null)
                        continue;
                    instances.Add(sagaInstance);
                }
                finally 
                {
                    kernel.ReleaseComponent(sagaPersister);
                }
            }
            return instances.ToArray();
        }
    }
}