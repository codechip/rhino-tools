using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqSubscriptionStorage : ISubscriptionStorage
    {
        private readonly Uri subscriptionQueue;
        private readonly Dictionary<string, List<WeakReference>> consumers = new Dictionary<string, List<WeakReference>>();

        private readonly Dictionary<string, HashSet<Uri>> subscriptions = new Dictionary<string, HashSet<Uri>>();
        private readonly Dictionary<TypeAndUriKey, IList<string>> subscriptionMessageIds = new Dictionary<TypeAndUriKey, IList<string>>();

        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IReflection reflection;
        private readonly IMessageSerializer messageSerializer;
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqSubscriptionStorage));

        public MsmqSubscriptionStorage(
            IReflection reflection,
            IMessageSerializer messageSerializer,
            Uri subscriptionQueue)
        {
            this.reflection = reflection;
            this.messageSerializer = messageSerializer;
            this.subscriptionQueue = subscriptionQueue;
            Initialize();
        }

        public void Initialize()
        {
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Receive))
            using (var enumerator = queue.GetMessageEnumerator2())
            {
                while (enumerator.MoveNext(TimeSpan.FromMilliseconds(0)))
                {
                    var current = enumerator.Current;
                    object[] msgs;
                    try
                    {
                        msgs = messageSerializer.Deserialize(new MsmqTransportMessage(current));
                    }
                    catch (Exception e)
                    {
                        throw new SubscriptionException("Could not deserialize message from subscription queue", e);
                    }

                    try
                    {
                        foreach (var msg in msgs)
                        {
                            HandleAdministrativeMessage(current.Id, msg);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new SubscriptionException("Failed to process subscription records", e);
                    }
                }
            }
        }

        private void AddMessageIdentifierForTracking(string messageId, string messageType, Uri uri)
        {
            var key = new TypeAndUriKey { TypeName = messageType, Uri = uri };
            IList<string> value;
            if (subscriptionMessageIds.TryGetValue(key, out value) == false)
            {
                subscriptionMessageIds[key] = value = new List<string>();
            }
            value.Add(messageId);
        }

        private void RemoveSubscriptionMessageFromQueue(MessageQueue queue, string type, Uri uri)
        {
            var key = new TypeAndUriKey
            {
                TypeName = type, Uri = uri
            };
            IList<string> messageIds;
            if (subscriptionMessageIds.TryGetValue(key, out messageIds) == false)
                return;
            foreach (var msgId in messageIds)
            {
                try
                {
                    queue.ReceiveById(msgId,
                                      TimeSpan.FromSeconds(0),
                                      MessageQueueTransactionType.Single);
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                        throw;
                }
            }
        }

        private static MessageQueue CreateSubscriptionQueue(Uri subscriptionQueue, QueueAccessMode accessMode)
        {
            var description = MsmqUtil.GetQueueDescription(subscriptionQueue);

            MessageQueue queue;
            try
            {
                queue = new MessageQueue(description.QueuePath, accessMode);
            }
            catch (Exception e)
            {
                throw new SubscriptionException("Could not open subscription queue (" + description.Uri + ")", e);
            }
            queue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
            return queue;
        }

        public IEnumerable<Uri> GetSubscriptionsFor(Type type)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                HashSet<Uri> subscriptionForType;
                if (subscriptions.TryGetValue(type.FullName, out subscriptionForType) == false)
                    return new Uri[0];
                return subscriptionForType;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        public object[] GetInstanceSubscriptions(Type type)
        {
            List<WeakReference> value;

            readerWriterLock.EnterReadLock();
            try
            {
                if (consumers.TryGetValue(type.FullName, out value) == false)
                    return new object[0];
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                var array = value
                    .Select(x => x.Target)
                    .Where(x => x != null)
                    .ToArray();

                value.RemoveAll(x => x.IsAlive == false);

                return array;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public DesiredMessageActionFromTransport HandleAdministrativeMessage(string messageId, object message)
        {
            var addSubscription = message as AddSubscription;
            if (addSubscription != null)
            {
                AddSubscription(addSubscription.Type, addSubscription.Endpoint);
                AddMessageIdentifierForTracking(messageId, addSubscription.Type, new Uri(addSubscription.Endpoint));
                return new DesiredMessageActionFromTransport
                {
                    Destination = "subscriptions",
                    MessageAction = MessageAction.Move
                };
            }
            var removeSubscription = message as RemoveSubscription;
            if (removeSubscription != null)
            {
                RemoveSubscription(removeSubscription.Type, removeSubscription.Endpoint);
                return new DesiredMessageActionFromTransport
                {
                    MessageAction = MessageAction.Consume
                };
            }
            return null;
        }

        public event Action SubscriptionChanged;

        public void AddSubscription(string type, string endpoint)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                HashSet<Uri> subscriptionsForType;
            if (subscriptions.TryGetValue(type, out subscriptionsForType) == false)
            {
                subscriptionsForType = new HashSet<Uri>();
                subscriptions[type] = subscriptionsForType;
            }

                var uri = new Uri(endpoint);
                subscriptionsForType.Add(uri);

                logger.InfoFormat("Added subscription for {0} on {1}",
                                  type, uri);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

            RaiseSubscriptionChanged();
        }

        private void RaiseSubscriptionChanged()
        {
            var copy = SubscriptionChanged;
            if (copy != null)
                copy();
        }

        public void RemoveSubscription(string type, string endpoint)
        {
            var uri = new Uri(endpoint);
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Receive))
            {
                RemoveSubscriptionMessageFromQueue(queue, type, uri);
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                HashSet<Uri> subscriptionsForType;

                if (subscriptions.TryGetValue(type, out subscriptionsForType) == false)
                {
                    subscriptionsForType = new HashSet<Uri>();
                    subscriptions[type] = subscriptionsForType;
                }

                subscriptionsForType.Remove(uri);

                logger.InfoFormat("Removed subscription for {0} on {1}",
                                  type, endpoint);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
            RaiseSubscriptionChanged();
        }

        public void AddInstanceSubscription(IMessageConsumer consumer)
        {
            var messagesConsumes = reflection.GetMessagesConsumed(consumer);

            readerWriterLock.EnterWriteLock();
            try
            {
                foreach (var type in messagesConsumes)
                {
                    List<WeakReference> value;
                    if (consumers.TryGetValue(type.FullName, out value) == false)
                    {
                        value = new List<WeakReference>();
                        consumers[type.FullName] = value;
                    }
                    value.Add(new WeakReference(consumer));
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
            RaiseSubscriptionChanged();
        }
    }
}