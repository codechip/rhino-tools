namespace SimpleDomainModel
{
    [System.SerializableAttribute()]
    public class OrderDetail
    {
        private float discount;

        private int productID;

        private short quantity;

        private decimal unitPrice;

        private Order order;

        public virtual Order Order
        {
            get { return order; }
            set { order = value; }
        }

        public virtual float Discount
        {
            get { return this.discount; }
            set { this.discount = value; }
        }

        public virtual short Quantity
        {
            get { return this.quantity; }
            set { this.quantity = value; }
        }


        public virtual int ProductID
        {
            get { return productID; }
            set { productID = value; }
        }

        public virtual decimal UnitPrice
        {
            get { return this.unitPrice; }
            set { this.unitPrice = value; }
        }

        public override bool Equals(object obj)
        {
            OrderDetail other = (OrderDetail)obj;
            return other.Order == Order && other.ProductID == ProductID;
        }

        public override int GetHashCode()
        {
            return Order.GetHashCode()*17 ^ ProductID;
        }
    }
}