using System;
using DSL.Demo.Model;

namespace DSL.Demo.Declerative
{
    public class ApplyDiscount : ICommand
    {
        private readonly decimal discountPrecentage;

        public ApplyDiscount(decimal discountPrecentage)
        {
            this.discountPrecentage = discountPrecentage;
        }

        public void Execute(Order order)
        {
            Console.WriteLine("Applying discount of {0:0%}, pay only: {1:C}", discountPrecentage,
                order.Total - (order.Total * discountPrecentage));
        }
    }
}