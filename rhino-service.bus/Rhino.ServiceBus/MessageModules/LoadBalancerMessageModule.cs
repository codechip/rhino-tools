using System;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;

namespace Rhino.ServiceBus.MessageModules
{
    public class LoadBalancerMessageModule : IMessageModule
    {
        private ITransport theTransport;

        private readonly Uri loadBalancerEndpoint;

        public LoadBalancerMessageModule(Uri loadBalancerEndpoint)
        {
            this.loadBalancerEndpoint = loadBalancerEndpoint;
        }

        public void Init(ITransport transport)
        {
            transport.MessageProcessingCompleted+=Transport_OnMessageProcessingCompleted;
            theTransport = transport;

            theTransport.Started += TellLoadBalancerThatWeAreReadyForWork;
        }

        private void Transport_OnMessageProcessingCompleted(CurrentMessageInformation t1, Exception t2)
        {
            TellLoadBalancerThatWeAreReadyForWork();
        }

        private void TellLoadBalancerThatWeAreReadyForWork()
        {
            theTransport.Send(loadBalancerEndpoint, new ReadyToWork
            {
                Endpoint = theTransport.Endpoint
            });
        }

        public void Stop(ITransport transport)
        {
            transport.MessageProcessingCompleted -= Transport_OnMessageProcessingCompleted;
            theTransport = null;
        }
    }
}