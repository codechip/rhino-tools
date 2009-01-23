using System.Messaging;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class ErrorDescriptionAction : AbstractTransportAction
    {
        private readonly IQueueStrategy queueStrategy;

        public ErrorDescriptionAction(IQueueStrategy queueStrategy)
        {
            this.queueStrategy = queueStrategy;
        }

        public override MessageType HandledType
        {
            get { return MessageType.ErrorDescriptionMessageMarker; }
        }

        public override bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            queueStrategy.MoveToErrorsQueue(queue, message);
            return true;
        }
    }
}