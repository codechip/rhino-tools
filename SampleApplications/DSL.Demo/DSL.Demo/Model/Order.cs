namespace DSL.Demo.Model
{
    public class Order
    {
        private static int counter = 0;
        private int id = ++counter;
        private decimal total;
        public Account Account { get; set; }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }
    }
}