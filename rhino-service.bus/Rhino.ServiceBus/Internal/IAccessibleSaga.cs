using System;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Internal
{
    public interface IAccessibleSaga : IMessageConsumer
    {
        Guid Id { get; set; }
        bool IsCompleted { get; set; }
    }
}