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
        protected MessageQueue queue;
        protected MessageQueue subscriptions;
        protected MessageQueue transactionalQueue;
        protected MessageQueue errorQueue;

        protected readonly Uri ErrorQueueUri;
        private readonly string errorTestQueuePath;

        protected readonly Uri TestQueueUri;
        private readonly string testQueuePath;

        protected readonly Uri SubscriptionsUri;
        private readonly string subbscriptionQueuePath;

        
        protected readonly Uri TransactionalTestQueueUri;
        private readonly string transactionalTestQueuePath;
        
        private ITransport transport;
        private ITransport transactionalTransport;

        public ITransport Transport
        {
            get
            {
                if (transport == null)
                {
                    transport = new MsmqTransport(new JsonSerializer(new DefaultReflection()), TestQueueUri,
                                                  ErrorQueueUri, 1, 5);
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
                    transactionalTransport = new MsmqTransport(new JsonSerializer(new DefaultReflection()), TransactionalTestQueueUri,
                                                               ErrorQueueUri, 1, 5);
                    transactionalTransport.Start();
                }
                return transactionalTransport;
            }
        }

        public MsmqTestBase()
        {
            ErrorQueueUri = new Uri("msmq://./error_test_queue");
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

            queue = new MessageQueue(testQueuePath);
            queue.Purge();

            errorQueue = new MessageQueue(errorTestQueuePath);
            var filter = new MessagePropertyFilter();
            filter.SetAll();
            errorQueue.MessageReadPropertyFilter = filter;
            errorQueue.Purge();

            transactionalQueue = new MessageQueue(transactionalTestQueuePath);
            transactionalQueue.Purge();

            subscriptions = new MessageQueue(subbscriptionQueuePath);
            subscriptions.Purge();
        }

        public void Dispose()
        {
            queue.Dispose();
            errorQueue.Dispose();
            transactionalQueue.Dispose();

            if(transport!=null)
                transport.Stop();
            if(transactionalTransport!=null)
                transactionalTransport.Stop();
        }
    }
}