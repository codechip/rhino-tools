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
        protected readonly Endpoint SubscriptionsUri;

        protected readonly string testQueuePath;
        protected readonly Endpoint TestQueueUri;
        
        protected readonly string transactionalTestQueuePath;
        protected readonly Endpoint TransactionalTestQueueUri;

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

            TestQueueUri = new Endpoint
            {
                Uri = new Uri("msmq://localhost/test_queue")
            };
            testQueuePath = MsmqUtil.GetQueuePath(TestQueueUri).QueuePath;

            TransactionalTestQueueUri = new Endpoint
            {
                Uri = new Uri("msmq://localhost/transactional_test_queue")
            };
			transactionalTestQueuePath = MsmqUtil.GetQueuePath(TransactionalTestQueueUri).QueuePath;

            SubscriptionsUri = new Endpoint
            {
                Uri = new Uri(TestQueueUri.Uri + "#" + subscriptions)
            };
			subscriptionQueuePath = MsmqUtil.GetQueuePath(SubscriptionsUri).QueuePath;


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
			ForEachQueuePath((path,transactional)=>
			             	{
								if (MessageQueue.Exists(path) == false)
								{
									MessageQueue.Create(path, transactional);
								}
								else
								{
									using (var cue = new MessageQueue(path))
									{
										cue.Purge();
									}
								}    		
			             	});
		}
		private void DeleteQueues()
		{
			ForEachQueuePath((path, transactional) =>
			{
				if (MessageQueue.Exists(path) )
				{
					MessageQueue.Delete(path);
				}
			});
		}
		private void ForEachQueuePath(Action<string,bool> action)
		{
			var rootUris = new Dictionary<string, bool>
			           	{
			           		{testQueuePath, false},
			           		{transactionalTestQueuePath, true}
			           	};
			var sub = new[] { "errors", "subscriptions", "discarded", "timeout" };
			foreach (var pair in rootUris)
			{
				foreach (var s in sub)
				{
					var path= pair.Key + "#" + s;
					action(path, pair.Value);
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
                        	new DefaultKernel()),
                            new FlatQueueStrategy(new EndpointRouter(),TestQueueUri.Uri),
                            TestQueueUri.Uri, 1,
                        DefaultTransportActions(TestQueueUri.Uri),
                            new EndpointRouter());
                    transport.Start();
                }
                return transport;
            }
        }

        private static ITransportAction[] DefaultTransportActions(Uri endpoint)
        {
            var qs = new FlatQueueStrategy(new EndpointRouter(),endpoint);
            return new ITransportAction[]
            {
                new AdministrativeAction(),
                new ErrorAction(5, qs),
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
                    transactionalTransport = new MsmqTransport(
                        new XmlMessageSerializer(new DefaultReflection(), new DefaultKernel()),
                        new FlatQueueStrategy(new EndpointRouter(),TransactionalTestQueueUri.Uri),
                        TransactionalTestQueueUri.Uri, 1, DefaultTransportActions(TransactionalTestQueueUri.Uri),
                            new EndpointRouter());
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
                transport.Dispose();
            if (transactionalTransport != null)
                transactionalTransport.Dispose();

			DeleteQueues();
        }

        #endregion
    }
}
