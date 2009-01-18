using System.Messaging;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public interface IMessageAction
    {
        void Init(IMsmqTrasport transport);

        bool CanHandlePeekedMessage(Message message);
        bool HandlePeekedMessage(MessageQueue queue, Message message);
    }
}