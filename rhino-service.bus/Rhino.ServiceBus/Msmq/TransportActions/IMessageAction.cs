using System.Messaging;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public interface IMessageAction
    {
        void Init(IMsmqTransport transport);

        bool CanHandlePeekedMessage(Message message);
        bool HandlePeekedMessage(MessageQueue queue, Message message);
    }
}