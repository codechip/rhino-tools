using System;

namespace Rhino.ServiceBus.LoadBalancer.Messages
{
    public class ReadyToWork
    {
        public Uri Endpoint { get; set; }
    }
}