using System;
using DSL.Demo.Declerative;
using DSL.Demo.Model;
using DSL.Demo.SampleDsl;
using Rhino.DSL;

namespace DSL.Demo
{
    public class Program
    {
        private static void Main2()
        {
            var factory = new DslFactory();
            factory.Register<SampleDslBase>(new SampleDslEngine());

            var dsl = factory.Create<SampleDslBase>("sample.dsl");
            dsl.Prepare();

            var entity = new Account();
            dsl.Creating(entity);
            Console.WriteLine("Account numer was set to: " + entity.AccountNumber);

            dsl.Creating(new Order
            {
                Account = new Account
                {
                    MaxOrderTotal = 10
                },
                Total = 15
            });
        }

        private static void Main()
        {
            var factory = new DslFactory();
            factory.Register<DeclerativeDslBase>(new DeclerativeDslEngine());

           while (true)
            {

                var dsl = factory.Create<DeclerativeDslBase>("Declerative.dsl");
                dsl.Prepare();
                
               dsl.Execute(
                    new Order { Total = 5000 },
                    new Customer { IsPreferred = false });
                dsl.Execute(
                    new Order { Total = 5000 },
                    new Customer { IsPreferred = true });

                dsl.Execute(
                   new Order { Total = 10 },
                   new Customer { IsPreferred = true });

                Console.ReadKey();
            }
        }
    }
}