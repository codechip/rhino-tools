namespace BDSLiB.MessageRouting.Handlers
{
    using Messages;

    public class NewOrderHandler : AbstractHandler<NewOrderMessage>
    {
        public override string Handle(NewOrderMessage message)
        {
            // do something with this message
            // here we just return a bogus message
            return string.Format("New Order Processed. For customer: {0}, TotalCost: {1}",
                                 message.CustomerId, message.OrderLines.Length*17);
        }
    }
}