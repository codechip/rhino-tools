namespace BDSLiB.MessageRouting.Messages
{
    public class NewOrderMessage
    {
        private readonly int customerId;
        private readonly string type;
        private readonly OrderLine[] orderLines;

        public int CustomerId
        {
            get { return customerId; }
        }

        public string Type
        {
            get { return type; }
        }

        public OrderLine[] OrderLines
        {
            get { return orderLines; }
        }

        public NewOrderMessage(int customerId, string type, OrderLine[] orderLines)
        {
            this.customerId = customerId;
            this.type = type;
            this.orderLines = orderLines;
        }
    }

    public class OrderLine
    {
        private readonly int productId;
        private readonly int quantity;

        public OrderLine(int productId, int quantity)
        {
            this.productId = productId;
            this.quantity = quantity;
        }

        public int ProductId
        {
            get { return productId; }
        }

        public int Quantity
        {
            get { return quantity; }
        }
    }
}