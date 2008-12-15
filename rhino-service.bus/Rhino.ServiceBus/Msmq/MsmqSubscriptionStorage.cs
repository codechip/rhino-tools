using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;

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
        private const string AddPrefix = "Add: ";
        private const string RemovePrefix = "Remove: ";
        private readonly ILog logger = LogManager.GetLogger(typeof(MsmqSubscriptionStorage));

        public MsmqSubscriptionStorage(IReflection reflection, Uri subscriptionQueue)
        {
            this.reflection = reflection;
            this.subscriptionQueue = subscriptionQueue;
            Initialize();
        }

        public void Initialize()
        {
            var list = new List<Action<MessageQueue>>();
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Receive))
            {
                using (var enumerator = queue.GetMessageEnumerator2())
                {
                    while (enumerator.MoveNext(TimeSpan.FromMilliseconds(0)))
                    {
                        var current = enumerator.Current;
                        var action = HandleSubscriptionFromMessage(current);
                        if (action != null)
                            list.Add(action);
                    }
                }
                using (var tx = new TransactionScope())
                {
                    foreach (var action in list)
                    {
                        action(queue);
                    }
                    tx.Complete();
                }
            }
        }

        private Action<MessageQueue> HandleSubscriptionFromMessage(Message current)
        {
            if (current == null)
                return null;
            var messageType = (string)current.Body;
            HashSet<Uri> subscriptionsForType;
            if (subscriptions.TryGetValue(messageType, out subscriptionsForType) == false)
            {
                subscriptionsForType = new HashSet<Uri>();
                subscriptions[messageType] = subscriptionsForType;
            }
            if (current.Label.StartsWith(AddPrefix))
            {
                var uri = new Uri(current.Label.Substring(AddPrefix.Length));
                subscriptionsForType.Add(uri);

                AddMessageIdentifierForTracking(current, messageType, uri);

                logger.InfoFormat("Added subscription for {0} on {1}",
                                  messageType, uri);
                return null;
            }
            if (current.Label.StartsWith(RemovePrefix))
            {
                var uri = new Uri(current.Label.Substring(RemovePrefix.Length));
                subscriptionsForType.Remove(uri);

                AddMessageIdentifierForTracking(current, messageType, uri);

                logger.InfoFormat("Removed subscription for {0} on {1}",
                                  messageType, uri);
                return queue => RemoveSubscriptionMessageFromQueue(queue, messageType, uri);
            }

            logger.WarnFormat("Could not understand subscription message '{0}' (ignoring)",
                              current.Label);
            return null;
        }

        private void AddMessageIdentifierForTracking(Message current, string messageType, Uri uri)
        {
            var key = new TypeAndUriKey { TypeName = messageType, Uri = uri };
            IList<string> value;
            if (subscriptionMessageIds.TryGetValue(key, out value) == false)
            {
                subscriptionMessageIds[key] = value = new List<string>();
            }
            value.Add(current.Id);
        }

        private void RemoveSubscriptionMessageFromQueue(MessageQueue queue, string type, Uri uri)
        {
            var key = new TypeAndUriKey { TypeName = type, Uri = uri };
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
            bool transactional;
            try
            {
                queue = new MessageQueue(description.QueuePath, accessMode);
                transactional = queue.Transactional;
            }
            catch (Exception e)
            {
                throw new SubscriptionException("Could not open subscription queue (" + description.Uri + ")", e);
            }

            if (transactional == false)
                throw new SubscriptionException("The subscription queue must be transactional (" +
                                                description.Uri + ")");

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

        public void AddSubscriptionIfNotExists(string type, Uri endpoint)
        {
            bool shouldRegister = true;

            readerWriterLock.EnterReadLock();
            try
            {

                HashSet<Uri> value;
                if (subscriptions.TryGetValue(type, out value))
                {
                    shouldRegister = value.Contains(endpoint); 
                }
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            if(shouldRegister)
                AddSubscription(type, endpoint.ToString());
        }

        public void AddSubscription(string type, string endpoint)
        {
            var message = new Message
            {
                Label = AddPrefix + endpoint,
                Body = type
            };
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Send))
            {
                queue.Send(message, MessageQueueTransactionType.Single);
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                HandleSubscriptionFromMessage(message);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void RemoveSubscription(string type, string endpoint)
        {
            var message = new Message
            {
                Label = RemovePrefix + endpoint,
                Body = type
            };
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Send))
            {
                queue.Send(message, MessageQueueTransactionType.Single);
            }

            readerWriterLock.EnterWriteLock();
            try
            {
                var action = HandleSubscriptionFromMessage(message);
                if (action != null)
                {
                    using (var tx = new TransactionScope())
                    {
                        action(CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Receive));
                        tx.Complete();
                    }
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
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
        }
    }

    public class TypeAndUriKey
    {
        public string TypeName;
        public Uri Uri;

        public bool Equals(TypeAndUriKey obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.TypeName, TypeName) && Equals(obj.Uri, Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(TypeAndUriKey)) return false;
            return Equals((TypeAndUriKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TypeName != null ? TypeName.GetHashCode() : 0) * 397) ^ (Uri != null ? Uri.GetHashCode() : 0);
            }
        }
    }
}