using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Sagas;
using Rhino.ServiceBus.Util;

namespace Rhino.ServiceBus.Impl
{
    public class DefaultServiceBus : IStartableServiceBus
    {
        private readonly IKernel kernel;

        private readonly ILog logger = LogManager.GetLogger(typeof(DefaultServiceBus));
        private readonly IMessageModule[] modules;
        private readonly IReflection reflection;
        private readonly ISubscriptionStorage subscriptionStorage;
        private readonly ITransport transport;
        private readonly MessageOwner[] messageOwners;

        public DefaultServiceBus(
            IKernel kernel,
            ITransport transport,
            ISubscriptionStorage subscriptionStorage,
            IReflection reflection,
            IMessageModule[] modules,
            MessageOwner[] messageOwners)
        {
            this.transport = transport;
            this.messageOwners = messageOwners;
            this.subscriptionStorage = subscriptionStorage;
            this.reflection = reflection;
            this.modules = modules;
            this.kernel = kernel;
        }

        public IMessageModule[] Modules
        {
            get { return modules; }
        }

        #region IStartableServiceBus Members

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
            if (messages == null)
                throw new ArgumentNullException("messages");

            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot reply with an empty message batch");

            transport.Reply(messages);
        }

        public void Send(Uri endpoint, params object[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");

            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot send empty message batch");

            transport.Send(endpoint, messages);
        }

        public void Send(params object[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");

            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot send empty message batch");

            bool sent = false;
            foreach (var owner in messageOwners)
            {
                if (owner.IsOwner(messages[0].GetType()) == false)
                    continue;

                Send(owner.Endpoint, messages);
                sent = true;
            }
            if (sent == false)
                throw new MessagePublicationException("Could not find no message owner for " + messages[0]);
        }

        public IDisposable AddInstanceSubscription(IMessageConsumer consumer)
        {
            subscriptionStorage.AddInstanceSubscription(consumer);
            var weakRef = new WeakReference(consumer);
            return new DisposableAction(() =>
            {
                var messageConsumer = weakRef.Target as IMessageConsumer;
                if (messageConsumer == null)//nothing to do
                    return;
                subscriptionStorage.RemoveInstanceSubscription(messageConsumer);
            });
        }

        public Uri Endpoint
        {
            get { return transport.Endpoint; }
        }

        public void Dispose()
        {
            transport.Stop();
            transport.MessageArrived -= Transport_OnMessageArrived;

            foreach (IMessageModule module in modules)
            {
                module.Stop(transport);
            }

            var subscriptionAsModule = subscriptionStorage as IMessageModule;
            if (subscriptionAsModule != null)
                subscriptionAsModule.Stop(transport);
        }

        public void Start()
        {
            logger.DebugFormat("Starting the bus for {0}", Endpoint);

            var subscriptionAsModule = subscriptionStorage as IMessageModule;
            if (subscriptionAsModule != null)
            {
                logger.DebugFormat("Initating subscription storage as message module: {0}", subscriptionAsModule);
                subscriptionAsModule.Init(transport);
            }
            foreach (var module in modules)
            {
                logger.DebugFormat("Initating message module: {0}", module);
                module.Init(transport);
            }
            transport.MessageArrived += Transport_OnMessageArrived;

            transport.Start();
            subscriptionStorage.Initialize();

            AutomaticallySubscribeConsumerMessages();
        }

        public void Subscribe(Type type)
        {
            foreach (var owner in messageOwners)
            {
                if (owner.IsOwner(type) == false)
                    continue;

                logger.InfoFormat("Subscribing {0} on {1}", type.FullName, owner.Endpoint);
                Send(owner.Endpoint, new AddSubscription
                {
                    Endpoint = Endpoint.ToString(),
                    Type = type.FullName
                });
            }
        }

        public void Subscribe<T>()
        {
            Subscribe(typeof(T));
        }

        public void Unsubscribe<T>()
        {
            Unsubscribe(typeof(T));
        }

        public void Unsubscribe(Type type)
        {
            foreach (var owner in messageOwners)
            {
                if (owner.IsOwner(type) == false)
                    continue;

                Send(owner.Endpoint, new RemoveSubscription
                {
                    Endpoint = Endpoint.ToString(),
                    Type = type.FullName
                });
            }
        }

        private void AutomaticallySubscribeConsumerMessages()
        {
            var handlers = kernel.GetAssignableHandlers(typeof(IMessageConsumer));
            foreach (var handler in handlers)
            {
                var msgs = reflection.GetMessagesConsumed(handler.ComponentModel.Implementation,
                                                          type => type == typeof(OccasionalConsumerOf<>));
                foreach (var msg in msgs)
                {
                    Subscribe(msg);
                }
            }
        }

        #endregion

        private bool PublishInternal(object[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");

            bool sentMsg = false;
            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot publish an empty message batch");
            object msg = messages[0];
            IEnumerable<Uri> subscriptions = subscriptionStorage.GetSubscriptionsFor(msg.GetType());
            foreach (Uri subscription in subscriptions)
            {
                transport.Send(subscription, messages);
                sentMsg = true;
            }
            return sentMsg;
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
                foreach (object consumer in consumers)
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
                foreach (object consumer in consumers)
                {
                    kernel.ReleaseComponent(consumer);
                }
            }
        }

        private void PersistSagaInstance(ISaga saga)
        {
            Type persisterType = reflection.GetGenericTypeOf(typeof(ISagaPersister<>), saga);
            object persister = kernel.Resolve(persisterType);

            if (saga.IsCompleted)
                reflection.InvokeSagaPersisterComplete(persister, saga);
            else
                reflection.InvokeSagaPersisterSave(persister, saga);
        }

        public object[] GatherConsumers(CurrentMessageInformation msg)
        {
            object[] sagas = GetSagasFor(msg.Message as ISagaMessage);

            object[] instanceConsumers = subscriptionStorage
                .GetInstanceSubscriptions(msg.Message.GetType());

            Type consumerType = reflection.GetGenericTypeOf(typeof(ConsumerOf<>), msg.Message);
            var consumers = (object[])kernel.ResolveAll(consumerType, new Hashtable());
            foreach (var consumer in consumers)
            {
                var saga = consumer as ISaga;
                if(saga==null)
                    continue;

                var type = saga.GetType();
                if (sagas.Any(type.IsInstanceOfType))
                    continue;

                saga.Id = GuidCombGenerator.Generate();
            }
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

            Type messageType = reflection.GetGenericTypeOf(typeof(Orchestrates<>), sagaMessage);

            IHandler[] handlers = kernel.GetAssignableHandlers(messageType);

            foreach (IHandler sagaPersisterHandler in handlers)
            {
                Type sagaPersisterType = reflection.GetGenericTypeOf(typeof(ISagaPersister<>),
                                                                     sagaPersisterHandler.ComponentModel.Implementation);

                object sagaPersister = kernel.Resolve(sagaPersisterType);
                try
                {
                    object sagaInstance = reflection.InvokeSagaPersisterGet(sagaPersister, sagaMessage.CorrelationId);
                    if (sagaInstance == null)
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