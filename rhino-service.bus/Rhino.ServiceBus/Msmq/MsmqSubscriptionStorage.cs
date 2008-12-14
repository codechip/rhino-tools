using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqSubscriptionStorage : ISubscriptionStorage
    {
        private readonly Dictionary<string, List<WeakReference>> consumers = new Dictionary<string, List<WeakReference>>();
        private readonly Dictionary<string, IList<Uri>> subscriptions = new Dictionary<string, IList<Uri>>();
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IReflection reflection;

        public MsmqSubscriptionStorage(IReflection reflection)
        {
            this.reflection = reflection;
        }

        public MsmqSubscriptionStorage(Uri subscriptionQueue)
        {
            using(var queue = CreateSubscriptionQueue(subscriptionQueue))
            {
                using(var enumerator = queue.GetMessageEnumerator2())
                {
                    while(enumerator.MoveNext(TimeSpan.FromMilliseconds(0)))
                    {
                        var current = enumerator.Current;
                        if (current == null)
                            continue;
                        var messageType = (string)current.Body;
                        IList<Uri> subscriptionsForType;
                        if(subscriptions.TryGetValue(messageType,out subscriptionsForType)==false)
                        {
                            subscriptionsForType = new List<Uri>();
                            subscriptions[messageType] = subscriptionsForType;
                        }
                        subscriptionsForType.Add(new Uri(current.Label));
                    }
                }
            }
        }

        private static MessageQueue CreateSubscriptionQueue(Uri subscriptionQueue)
        {
            var description = MsmqUtil.GetQueueDescription(subscriptionQueue);

            MessageQueue queue;
            bool transactional;
            try
            {
                queue = new MessageQueue(description.QueuePath,QueueAccessMode.Receive);
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
                IList<Uri> subscriptionForType;
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
                if(consumers.TryGetValue(type.FullName, out value)==false)
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
                    .Select(x=>x.Target)
                    .Where(x=>x!=null)
                    .ToArray();

                value.RemoveAll(x => x.IsAlive == false);

                return array;
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public void AddInstanceSubscription(IMessageConsumer consumer)
        {
            var messagesConsumes = reflection.GetMessagesConsumes(consumer);

            readerWriterLock.EnterWriteLock();
            try
            {
                foreach (var type in messagesConsumes)
                {
                    List<WeakReference> value;
                    if(consumers.TryGetValue(type.FullName,out value)==false)
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
}