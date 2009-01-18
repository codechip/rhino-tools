using System.Messaging;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class DiscardAction : AbstractMessageAction
    {
        private readonly IQueueStrategy queueStrategy;

        public DiscardAction(IQueueStrategy queueStrategy)
        {
            this.queueStrategy = queueStrategy;
        }

        public override MessageType HandledType
        {
            get { return MessageType.DiscardedMessageMarker; }
        }

        public override bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            queueStrategy.MoveToDiscardedQueue(queue, message);
            return true;
        }
    }
}