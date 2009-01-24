using System;

namespace Rhino.ServiceBus.LoadBalancer
{
    public interface ILoadBalancer : IDisposable
    {
        void Start();
    }
}