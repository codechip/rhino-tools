using System;

namespace Rhino.ServiceBus.Messages
{
    public class MessageArrivedMessage
    {
        public Uri Source { get; set; }

        public DateTime Timestamp { get; set; }

        public string MessageType { get; set; }

        public string MessageId { get; set; }

        public string CorrelationId { get; set; }

        public object Message { get; set; }
    }
}