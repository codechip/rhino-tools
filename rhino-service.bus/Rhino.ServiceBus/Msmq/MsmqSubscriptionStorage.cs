using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Messages;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqSubscriptionStorage : ISubscriptionStorage, IMessageModule
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
            this.subscriptionQueue = new Uri(subscriptionQueue + ";subscriptions");
        }

        public void Initialize()
        {
            logger.DebugFormat("Initializing msmq subscription storage on: {0}", subscriptionQueue);
            using (var queue = CreateSubscriptionQueue(subscriptionQueue, QueueAccessMode.Receive))
            using (var enumerator = queue.GetMessageEnumerator2())
            {
                while (enumerator.MoveNext(TimeSpan.FromMilliseconds(0)))
                {
                    var current = enumerator.Current;
                    if (current == null)
                        continue;
                    object[] msgs;
                    try
                    {
                        msgs = messageSerializer.Deserialize(current.BodyStream);
                    }
                    catch (Exception e)
                    {
                        throw new SubscriptionException("Could not deserialize message from subscription queue", e);
                    }

                    try
                    {
                        foreach (var msg in msgs)
                        {
                            HandleAdministrativeMessage(new CurrentMessageInformation
                            {
                                AllMessages = msgs,
                                CorrelationId = CorrelationId.Empty,
                                Message = msg,
                                MessageId = CorrelationId.Parse(current.Id),
                                Source = subscriptionQueue,
                            });
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
                TypeName = type,
                Uri = uri
            };
            IList<string> messageIds;
            if (subscriptionMessageIds.TryGetValue(key, out messageIds) == false)
                return;
            foreach (var msgId in messageIds)
            {
                try
                {
                    queue.ReceiveById(msgId,
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

        public void RemoveInstanceSubscription(IMessageConsumer consumer)
        {
            var messagesConsumes = reflection.GetMessagesConsumed(consumer);
            bool changed = false;
            readerWriterLock.EnterWriteLock();
            try
            {
                foreach (var type in messagesConsumes)
                {
                    List<WeakReference> value;

                    if (consumers.TryGetValue(type.FullName, out value) == false)
                        continue;

                    if (value.RemoveAll(x => ReferenceEquals(x.Target, consumer)) > 0)
                        changed = true;
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
            if (changed)
                RaiseSubscriptionChanged();
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

        public void HandleAdministrativeMessage(CurrentMessageInformation msgInfo)
        {
            var msmqMsgInfo = msgInfo as MsmqCurrentMessageInformation;
            var addSubscription = msgInfo.Message as AddSubscription;
            if (addSubscription != null)
            {
                bool newSubscription = AddSubscription(addSubscription.Type, addSubscription.Endpoint);
                AddMessageIdentifierForTracking(msgInfo.MessageId.ToString(), addSubscription.Type, new Uri(addSubscription.Endpoint));
                if (msmqMsgInfo != null)
                {
                    if (newSubscription)
                        msmqMsgInfo.Queue.MoveToSubQueue("subscriptions", msmqMsgInfo.MsmqMessage);
                    else
                        ConsumeMessageFromQueue(msmqMsgInfo);
                }
                return;
            }
            var removeSubscription = msgInfo.Message as RemoveSubscription;
            if (removeSubscription == null)
                return;
            RemoveSubscription(removeSubscription.Type, removeSubscription.Endpoint);
            ConsumeMessageFromQueue(msmqMsgInfo);
        }

        private static void ConsumeMessageFromQueue(MsmqCurrentMessageInformation msmqMsgInfo)
        {
            if (msmqMsgInfo == null)
                return;
                
            try
            {
                msmqMsgInfo.Queue.ReceiveById(
                    msmqMsgInfo.MsmqMessage.Id,
                    msmqMsgInfo.TransactionType);
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    throw;
            }
        }

        public event Action SubscriptionChanged;

        public bool AddSubscription(string type, string endpoint)
        {
            bool added;
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
                added = subscriptionsForType.Add(uri);

                logger.InfoFormat("Added subscription for {0} on {1}",
                                  type, uri);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

            RaiseSubscriptionChanged();
            return added;
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

        void IMessageModule.Init(ITransport transport)
        {
            transport.AdministrativeMessageArrived += HandleAdministrativeMessage;
        }

        void IMessageModule.Stop(ITransport transport)
        {
            transport.AdministrativeMessageArrived -= HandleAdministrativeMessage;
        }
    }
}