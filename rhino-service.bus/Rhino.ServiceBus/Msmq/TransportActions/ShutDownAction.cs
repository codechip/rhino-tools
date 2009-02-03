using System.Messaging;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class ShutDownAction : AbstractTransportAction
    {
        public override MessageType HandledType
        {
            get { return MessageType.ShutDownMessageMarker; }
        }

        public override bool HandlePeekedMessage(OpenedQueue queue, Message message)
        {
            queue.TryGetMessageFromQueue(message.Id);
            return true;
        }
    }
}