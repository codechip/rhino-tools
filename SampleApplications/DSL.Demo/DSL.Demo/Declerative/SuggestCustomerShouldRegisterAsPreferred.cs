using System;
using DSL.Demo.Model;

namespace DSL.Demo.Declerative
{
    public class SuggestCustomerShouldRegisterAsPreferred : ICommand
    {
        public void Execute(Order order)
        {
            Console.WriteLine("Would you like to be a preferred customer?");
        }
    }
}