using System;
using System.Collections.Generic;
using DSL.Demo.Model;

namespace DSL.Demo.Declerative
{
    public abstract class DeclerativeDslBase
    {
        public delegate bool ShouldExecuteCommand(Order order, Customer customer);
        public delegate ShouldExecuteCommand GeneratePredicate();

        private readonly Dictionary<ShouldExecuteCommand, ICommand> predicatesToCommands =
            new Dictionary<ShouldExecuteCommand, ICommand>();

        public abstract void Prepare();

        public void suggest_register_as_preferred(GeneratePredicate generatePredicate)
        {
            predicatesToCommands[generatePredicate()] = 
                new SuggestCustomerShouldRegisterAsPreferred();
        }

        public void apply_discount(decimal discountPrecentage, GeneratePredicate generatePredicate)
        {
            predicatesToCommands[generatePredicate()] = new ApplyDiscount(discountPrecentage);
        }

        public void deny_sale(GeneratePredicate generatePredicate)
        {
            predicatesToCommands[generatePredicate()] = new DenySale();
        }

        public void Execute(Order order, Customer customer)
        {
            foreach (var pair in predicatesToCommands)
            {
                if(pair.Key(order, customer))
                    pair.Value.Execute(order);
            }
        }
    }

    internal class DenySale
        : ICommand
    {
        public void Execute(Order order)
        {
            Console.WriteLine("deny sale");
        }
    }
}