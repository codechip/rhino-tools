using System;

namespace Starbucks.Messages
{
    public class PaymentDue
    {
        public string CustomerName { get; set; }
        public Guid StarbucksTransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}