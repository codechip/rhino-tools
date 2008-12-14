using System;
using System.Collections.Generic;

namespace Rhino.ServiceBus.Internal
{
    public interface ISubscriptionStorage
    {
        IEnumerable<Uri> GetSubscriptionsFor(Type type);
        void AddInstanceSubscription(IMessageConsumer consumer);
        object[] GetInstanceSubscriptions(Type type);
    }
}