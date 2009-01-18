using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class AdministrativeAction : AbstractMessageAction
    {
        private IMsmqTrasport transport;

        public override MessageType HandledType
        {
            get { return MessageType.AdministrativeMessageMarker; }
        }

        public override void Init(IMsmqTrasport parentTransport)
        {
            transport = parentTransport;
        }

        public override bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            Action<CurrentMessageInformation> messageRecieved = information =>
            {
                var msmqCurrentMessageInformation = (MsmqCurrentMessageInformation)information;
                var messageProcessedCorrectly = transport.RaiseAdministrativeMessageArrived(information);

                if (messageProcessedCorrectly)
                    return;
              
                //consume unknown message
                msmqCurrentMessageInformation.Queue
                    .ConsumeMessage(msmqCurrentMessageInformation.MsmqMessage);
            };

            transport.ProcessMessage(message, queue, null, 
                messageRecieved,
                transport.RaiseAdministrativeMessageProcessingCompleted);

            return true;
        }
    }
}