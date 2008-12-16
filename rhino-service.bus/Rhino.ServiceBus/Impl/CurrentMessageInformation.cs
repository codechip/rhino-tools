using System;

namespace Rhino.ServiceBus.Impl
{
    public class CurrentMessageInformation
    {
        public CorrelationId MessageId { get; set; }

        public CorrelationId CorrelationId { get; set; }

        public Uri Source { get; set; }

        public object Message { get; set; }

        public object[] AllMessages { get; set; }
    }
}
