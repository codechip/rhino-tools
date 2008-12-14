using System.IO;
using System.Messaging;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Msmq
{
    public class MsmqTransportMessage : ITransportMessage
    {
        private readonly Message message;

        public MsmqTransportMessage(Message message)
        {
            this.message = message;
        }

        #region ITransportMessage Members

        public Stream BodyStream
        {
            get { return message.BodyStream; }
            set { message.BodyStream = value; }
        }

        #endregion
    }
}