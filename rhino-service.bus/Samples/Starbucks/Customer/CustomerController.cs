using System;
using System.Threading;
using Rhino.ServiceBus;
using Starbucks.Messages;
using Starbucks.Messages.Cashier;

namespace Starbucks.Customer
{
    public class CustomerController : 
        ConsumerOf<PaymentDue>,
        ConsumerOf<DrinkReady>
    {
        public string Name { get; set; }
        public string Drink { get; set; }
        public DrinkSize Size { get; set; }
        private readonly IServiceBus bus;
        private ManualResetEvent wait;

        public CustomerController(IServiceBus bus)
        {
            this.bus = bus;
        }

        public void BuyDrinkSync()
        {
            using(bus.AddInstanceSubscription(this))
            {
                wait = new ManualResetEvent(false);

                bus.Send(new NewOrder {CustomerName = Name, DrinkName = Drink, Size = Size});

                wait.WaitOne();
            }
        }

        public void Consume(PaymentDue message)
        {
            Console.WriteLine("Payment due is: {0:D} y/n?", message.Amount);
            if(Console.ReadKey().KeyChar!='y')
                return;

            bus.Reply(new SubmitPayment
            {
                Amount = message.Amount, 
                CorrelationId = message.StarbucksTransactionId
            });
        }

        public void Consume(DrinkReady message)
        {
            Console.WriteLine("{0}: Got the drink, coffee rush!", Name);
            if (wait != null)
                wait.Set();
        }
    }
}