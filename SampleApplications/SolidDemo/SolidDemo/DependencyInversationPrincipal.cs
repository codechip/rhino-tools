namespace SolidDemo
{
    public class OrderProcessor
    {
        public decimal CalculateTotal(PurchaseOrder order)
        {
            decimal itemTotal = order.GetItemTotal();
            decimal discountAmount = CalculateDiscount(order);

            decimal taxAmount = 0.0M;

            if (order.Country == "US")
                taxAmount = FindTaxAmount(order);
            else if (order.Country == "UK")
                taxAmount = FindVatAmount(order);

            decimal total = itemTotal - discountAmount + taxAmount;

            return total;
        }

        private decimal CalculateDiscount(PurchaseOrder order)
        {
            if (order.GetItemTotal() > 3)
                return 10m;
            return 0m;
        }

        private decimal FindVatAmount(PurchaseOrder order)
        {
            // find the UK value added tax somehow
            return 10.0M;
        }

        private decimal FindTaxAmount(PurchaseOrder order)
        {
            // find the US tax somehow
            return 10.0M;
        }
    }

    public class PurchaseOrder
    {
        public string Country;

        public decimal GetItemTotal()
        {
            throw new System.NotImplementedException();
        }
    }
}