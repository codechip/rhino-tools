using System;
using System.Collections.Generic;

namespace Rhino.ServiceBus.Internal
{
    public interface ISubscriptionStorage
    {
        void Initialize();

        IEnumerable<Uri> GetSubscriptionsFor(Type type);
        
        void AddInstanceSubscription(IMessageConsumer consumer);

        void RemoveInstanceSubscription(IMessageConsumer consumer);
        
        object[] GetInstanceSubscriptions(Type type);

        void AddSubscription(string type, string endpoint);

        void RemoveSubscription(string type, string endpoint);

        event Action SubscriptionChanged;
    }
}