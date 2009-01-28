using System;
using System.Messaging;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus.Tests.LoadBalancer
{
    public class LoadBalancingTestBase : MsmqTestBase
    {
        protected const string loadBalancerQueue = "msmq://localhost/test_queue.balancer";

        public LoadBalancingTestBase()
        {
            var queuePath = MsmqUtil.GetQueuePath(new Uri(loadBalancerQueue).ToEndpoint());
            if (MessageQueue.Exists(queuePath) == false)
                MessageQueue.Create(queuePath, true);
            using (var loadBalancer = new MessageQueue(queuePath, QueueAccessMode.SendAndReceive))
            {
                loadBalancer.Purge();
            }

            using (var loadBalancer = new MessageQueue(queuePath + ";Workers", QueueAccessMode.SendAndReceive))
            {
                loadBalancer.Purge();
            }
 
        }
    }
}