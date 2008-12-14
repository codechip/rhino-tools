using System;

namespace Rhino.ServiceBus.Sagas
{
    public interface ISaga : IMessageConsumer
    {
        Guid Id { get; set; }
        bool IsCompleted { get; set; }
    }
}