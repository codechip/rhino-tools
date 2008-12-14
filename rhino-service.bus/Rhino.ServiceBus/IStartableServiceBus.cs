using System;

namespace Rhino.ServiceBus
{
    public interface IStartableServiceBus : IServiceBus, IDisposable
    {
        void Start();
    }
}