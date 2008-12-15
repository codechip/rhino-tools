using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqTestBase : IDisposable
    {
        protected readonly Uri ErrorQueueUri;
        private readonly string errorTestQueuePath;

        private readonly string subbscriptionQueuePath;
        protected readonly Uri SubscriptionsUri;

        private readonly string managementQueuePath;
        protected readonly Uri ManagementUri;

        private readonly string testQueuePath;
        protected readonly Uri TestQueueUri;


        private readonly string transactionalTestQueuePath;
        protected readonly Uri TransactionalTestQueueUri;
        protected MessageQueue errorQueue;
        protected MessageQueue queue;
        protected MessageQueue subscriptions;
        protected MessageQueue management;
        protected MessageQueue transactionalQueue;

        private ITransport transactionalTransport;
        private ITransport transport;

        public MsmqTestBase()
        {
            ManagementUri = new Uri("msmq://./test_queue_management");
            managementQueuePath = MsmqUtil.GetQueueDescription(ManagementUri).QueuePath;

            ErrorQueueUri = new Uri("msmq://./test_queue_error");
            errorTestQueuePath = MsmqUtil.GetQueueDescription(ErrorQueueUri).QueuePath;

            TestQueueUri = new Uri("msmq://./test_queue");
            testQueuePath = MsmqUtil.GetQueueDescription(TestQueueUri).QueuePath;

            TransactionalTestQueueUri = new Uri("msmq://./transactional_test_queue");
            transactionalTestQueuePath = MsmqUtil.GetQueueDescription(TransactionalTestQueueUri).QueuePath;

            SubscriptionsUri = new Uri("msmq://./test_subscriptions");
            subbscriptionQueuePath = MsmqUtil.GetQueueDescription(SubscriptionsUri).QueuePath;

            if (MessageQueue.Exists(testQueuePath) == false)
                MessageQueue.Create(testQueuePath);

            if (MessageQueue.Exists(errorTestQueuePath) == false)
                MessageQueue.Create(errorTestQueuePath);

            if (MessageQueue.Exists(transactionalTestQueuePath) == false)
                MessageQueue.Create(transactionalTestQueuePath, true);

            if (MessageQueue.Exists(subbscriptionQueuePath) == false)
                MessageQueue.Create(subbscriptionQueuePath, true);

            if (MessageQueue.Exists(managementQueuePath) == false)
                MessageQueue.Create(managementQueuePath, true);

            queue = new MessageQueue(testQueuePath);
            queue.Purge();

            management = new MessageQueue(managementQueuePath);
            management.Purge();

            errorQueue = new MessageQueue(errorTestQueuePath);
            var filter = new MessagePropertyFilter();
            filter.SetAll();
            errorQueue.MessageReadPropertyFilter = filter;
            errorQueue.Purge();

            transactionalQueue = new MessageQueue(transactionalTestQueuePath);
            transactionalQueue.Purge();

            subscriptions = new MessageQueue(subbscriptionQueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] {typeof (string)})
            };
            subscriptions.Purge();
        }

        public ITransport Transport
        {
            get
            {
                if (transport == null)
                {
                    transport = new MsmqTransport(new JsonSerializer(new DefaultReflection()), TestQueueUri,
                                                  SubscriptionsUri, ErrorQueueUri, 1, 5);
                    transport.Start();
                }
                return transport;
            }
        }

        public ITransport TransactionalTransport
        {
            get
            {
                if (transactionalTransport == null)
                {
                    transactionalTransport = new MsmqTransport(new JsonSerializer(new DefaultReflection()),
                                                               TransactionalTestQueueUri,
                                                               SubscriptionsUri, ErrorQueueUri, 1, 5);
                    transactionalTransport.Start();
                }
                return transactionalTransport;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            queue.Dispose();
            errorQueue.Dispose();
            transactionalQueue.Dispose();

            if (transport != null)
                transport.Stop();
            if (transactionalTransport != null)
                transactionalTransport.Stop();
        }

        #endregion
    }
}