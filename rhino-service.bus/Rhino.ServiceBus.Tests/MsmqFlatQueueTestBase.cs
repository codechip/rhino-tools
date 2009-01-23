using System;
using System.Collections.Generic;
using System.Messaging;
using Castle.MicroKernel;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Msmq.TransportActions;
using Rhino.ServiceBus.Serializers;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqFlatQueueTestBase : IDisposable
    {
        private readonly string subscriptionQueuePath;
        protected readonly Uri SubscriptionsUri;

        protected readonly string testQueuePath;
        protected readonly Uri TestQueueUri;
        
        protected readonly string transactionalTestQueuePath;
        protected readonly Uri TransactionalTestQueueUri;

        protected MessageQueue queue;
        protected MessageQueue subscriptions;
        protected MessageQueue transactionalQueue;

        private ITransport transactionalTransport;
        private ITransport transport;

        /// <summary>
        /// we use this to initalize the defaults for the test
        /// </summary>
        private readonly MsmqTestBase defaultTestBase;

        public MsmqFlatQueueTestBase()
        {
            defaultTestBase = new MsmqTestBase();

            TestQueueUri = new Uri("msmq://localhost/test_queue");
            testQueuePath = MsmqUtil.GetQueuePath(TestQueueUri);
            
            TransactionalTestQueueUri = new Uri("msmq://localhost/transactional_test_queue");
            transactionalTestQueuePath = MsmqUtil.GetQueuePath(TransactionalTestQueueUri);

			SubscriptionsUri = new Uri(TestQueueUri + "#" + subscriptions);
			subscriptionQueuePath = MsmqUtil.GetQueuePath(SubscriptionsUri);


			SetupQueues();

            queue = new MessageQueue(testQueuePath);
            transactionalQueue = new MessageQueue(transactionalTestQueuePath);
            subscriptions = new MessageQueue(subscriptionQueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(string) })
            };
        }
		
		private void SetupQueues()
		{
			var rootUris = new Dictionary<string, bool>
			           	{
			           		{testQueuePath, false},
			           		{transactionalTestQueuePath, true}
			           	};
			var sub = new[] {"errors", "subscriptions", "discarded", "timeout"};
			foreach (var pair in rootUris)
			{
				foreach (var s in sub)
				{
					var q = pair.Key + "#" + s;
					if (MessageQueue.Exists(q) == false)
					{
						MessageQueue.Create(q,pair.Value);
					}
					else
					{
						using (var cue = new MessageQueue(q))
						{
							cue.Purge();
						}
					}
				}

			}
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
                        	new DefaultKernel()), TestQueueUri, 1,
                        DefaultMessageActions(TestQueueUri), new FlatQueueStrategy(TestQueueUri));
                    transport.Start();
                }
                return transport;
            }
        }

        private IMessageAction[] DefaultMessageActions(Uri endpoint)
        {
            var qs = new FlatQueueStrategy(endpoint);
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

        public ITransport TransactionalTransport
        {
            get
            {
                if (transactionalTransport == null)
                {
                    transactionalTransport = new MsmqTransport(new XmlMessageSerializer(new DefaultReflection(), new DefaultKernel()),
                                                               TransactionalTestQueueUri, 1, DefaultMessageActions(TransactionalTestQueueUri), new FlatQueueStrategy(TransactionalTestQueueUri));
                    transactionalTransport.Start();
                }
                return transactionalTransport;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            defaultTestBase.Dispose();

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
