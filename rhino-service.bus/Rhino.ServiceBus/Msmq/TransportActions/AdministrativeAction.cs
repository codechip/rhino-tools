using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class AdministrativeAction : AbstractTransportAction
    {
        private IMsmqTransport transport;

        public override MessageType HandledType
        {
            get { return MessageType.AdministrativeMessageMarker; }
        }

        public override void Init(IMsmqTransport parentTransport)
        {
            transport = parentTransport;
        }

        public override bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            Func<CurrentMessageInformation, bool> messageRecieved = information =>
            {
                var msmqCurrentMessageInformation = (MsmqCurrentMessageInformation)information;
                var messageProcessedCorrectly = transport.RaiseAdministrativeMessageArrived(information);

                if (messageProcessedCorrectly)
                    return true;
              
                //consume unknown message
                msmqCurrentMessageInformation.Queue
                    .ConsumeMessage(msmqCurrentMessageInformation.MsmqMessage);

                return true;
            };

            transport.ProcessMessage(message, queue, null, 
                messageRecieved,
                transport.RaiseAdministrativeMessageProcessingCompleted);

            return true;
        }
    }
}