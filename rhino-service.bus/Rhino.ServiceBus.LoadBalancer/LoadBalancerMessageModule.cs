using System;
using System.Diagnostics;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer.Messages;
using Rhino.ServiceBus.MessageModules;

namespace Rhino.ServiceBus.LoadBalancer
{
    public class LoadBalancerMessageModule : IMessageModule
    {
        private readonly PerformanceCounter cpuTime = new PerformanceCounter("Process", "% Processor Time", "_Total", true);
        private ITransport theTransport;

        private Uri loadBalancerEndpoint;

        public LoadBalancerMessageModule(Uri loadBalancerEndpoint)
        {
            this.loadBalancerEndpoint = loadBalancerEndpoint;
        }

        public void Init(ITransport transport)
        {
            transport.MessageProcessingCompleted+=Transport_OnMessageProcessingCompleted;
            theTransport = transport;
        }

        private void Transport_OnMessageProcessingCompleted(CurrentMessageInformation t1, Exception t2)
        {
            var value = cpuTime.NextValue();
            if(value > 80.1)//note that 80% means GC collection in progress, so we ignore that
                return;

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