using System.Messaging;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class ShutDownAction : AbstractMessageAction
    {
        public override MessageType HandledType
        {
            get { return MessageType.ShutDownMessageMarker; }
        }

        public override bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            queue.TryGetMessageFromQueue(message.Id);
            return true;
        }
    }
}