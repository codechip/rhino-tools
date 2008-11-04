namespace SolidDemo
{
    public class TaxCalculator
    {
        public decimal CalculateTax(Order order)
        {
            if (order.Address.ZipCode == "123456")
            {
                return order.Total * 0.2m;
            }
            return order.Total * 0.1m;
        }
    }

    public class Order
    {
        public decimal Total;
        public Address Address { get; set; }
    }

    public class Address
    {
        public string ZipCode { get; set; }
    }
}