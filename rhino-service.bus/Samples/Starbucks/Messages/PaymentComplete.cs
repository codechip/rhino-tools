using System;
using Rhino.ServiceBus.Sagas;

namespace Starbucks.Messages
{
    public class PaymentComplete : ISagaMessage
    {
        public Guid CorrelationId { get; set; }
        
    }
}