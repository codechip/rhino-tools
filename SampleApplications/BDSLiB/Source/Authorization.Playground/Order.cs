namespace Chapter5.Security
{
    using System.Diagnostics;
    using System.Threading;

    public class Order
    {
        private decimal totalCost;

        public decimal TotalCost
        {
            get
            {
                return totalCost;
            }
            set { totalCost = value; }
        }
    }
}