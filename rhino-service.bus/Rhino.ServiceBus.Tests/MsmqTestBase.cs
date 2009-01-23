using System;
using System.Messaging;
using Castle.MicroKernel;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Msmq.TransportActions;
using Rhino.ServiceBus.Serializers;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqTestBase : IDisposable
    {
        private readonly string subbscriptionQueuePath;
        protected readonly Uri SubscriptionsUri;

        protected readonly string testQueuePath;
        protected readonly Uri TestQueueUri;

        protected readonly string testQueuePath2;
        protected readonly Uri TestQueueUri2;

        protected readonly string transactionalTestQueuePath;
        protected readonly Uri TransactionalTestQueueUri;

        protected MessageQueue queue;
        protected MessageQueue subscriptions;
        protected MessageQueue transactionalQueue;

        private ITransport transactionalTransport;
        private ITransport transport;
        protected readonly MessageQueue testQueue2;

        public MsmqTestBase()
        {
            TestQueueUri = new Uri("msmq://localhost/test_queue");
            testQueuePath = MsmqUtil.GetQueuePath(TestQueueUri);

            TestQueueUri2 = new Uri("msmq://localhost/test_queue2");
            testQueuePath2 = MsmqUtil.GetQueuePath(TestQueueUri2);

            TransactionalTestQueueUri = new Uri("msmq://localhost/transactional_test_queue");
            transactionalTestQueuePath = MsmqUtil.GetQueuePath(TransactionalTestQueueUri);

            SubscriptionsUri = new Uri("msmq://localhost/test_queue;subscriptions");
            subbscriptionQueuePath = MsmqUtil.GetQueuePath(SubscriptionsUri);

            if (MessageQueue.Exists(testQueuePath) == false)
                MessageQueue.Create(testQueuePath);

            if (MessageQueue.Exists(testQueuePath2) == false)
                MessageQueue.Create(testQueuePath2);

            if (MessageQueue.Exists(transactionalTestQueuePath) == false)
                MessageQueue.Create(transactionalTestQueuePath, true);

            queue = new MessageQueue(testQueuePath);
            queue.Purge();

            using (var errQueue = new MessageQueue(testQueuePath + ";errors"))
            {
                errQueue.Purge();
            }

            testQueue2 = new MessageQueue(testQueuePath2);
            testQueue2.Purge();

            using (var errQueue2 = new MessageQueue(testQueuePath2 + ";errors"))
            {
                errQueue2.Purge();
            }

            transactionalQueue = new MessageQueue(transactionalTestQueuePath);
            transactionalQueue.Purge();

            using (var errQueue3 = new MessageQueue(transactionalTestQueuePath + ";errors"))
            {
                errQueue3.Purge();
            }

            using (var discardedQueue = new MessageQueue(testQueuePath + ";discarded"))
            {
                discardedQueue.Purge();
            }

			using (var timeoutQueue = new MessageQueue(testQueuePath + ";timeout"))
			{
				timeoutQueue.Purge();
			}

            subscriptions = new MessageQueue(subbscriptionQueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(string) })
            };
            subscriptions.Purge();
        }

        public ITransport Transport
        {
            get
            {
                if (transport == null)
                {
                    transport = new MsmqTransport(
                        new XmlMessageSerializer(
                        	new DefaultReflection(), 
                        	new DefaultKernel()), 
                            TestQueueUri, 1, 
                            DefaultMessageActions, new SubQueueStrategy());
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
                    transactionalTransport = new MsmqTransport(new XmlMessageSerializer(new DefaultReflection(), new DefaultKernel()),
                                                               TransactionalTestQueueUri, 1, DefaultMessageActions, new SubQueueStrategy());
                    transactionalTransport.Start();
                }
                return transactionalTransport;
            }
        }

        private static IMessageAction[] DefaultMessageActions
        {
            get
            {
                var qs = new SubQueueStrategy();
                return new IMessageAction[]
                {
                    new AdministrativeAction(),
                    new DiscardAction(qs),
                    new ErrorAction(5, qs),
                    new ErrorDescriptionAction(qs),
                    new ShutDownAction(),
                    new TimeoutAction(qs)
                };
            }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            queue.Dispose();
            transactionalQueue.Dispose();
            subscriptions.Dispose();

            if (transport != null)
                transport.Stop();
            if (transactionalTransport != null)
                transactionalTransport.Stop();
        }

        #endregion
    }
}