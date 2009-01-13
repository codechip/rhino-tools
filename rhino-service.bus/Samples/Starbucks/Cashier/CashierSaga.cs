using System;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Sagas;
using Starbucks.Messages.Cashier;

namespace Starbucks.Cashier
{
    public class CashierSaga :
        InitiatedBy<NewOrder>,
        Orchestrates<SubmitPayment>
    {
        private readonly IServiceBus bus;

        public CashierSaga(IServiceBus bus)
        {
            this.bus = bus;
        }

        #region InitiatedBy<NewOrder> Members

        public void Consume(NewOrder message)
        {
            Console.WriteLine("Cashier: got new order");
            bus.Publish(new PrepareDrink
            {
                CorrelationId = Id,
                CustomerName = message.CustomerName,
                DrinkName = message.DrinkName,
                Size = message.Size
            });
            bus.Reply(new PaymentDue
            {
                CustomerName = message.CustomerName,
                StarbucksTransactionId = Id,
                Amount = ((int) message.Size)*1.25m
            });
        }

        public Guid Id { get; set; }

        public bool IsCompleted { get; set; }

        #endregion

        #region Orchestrates<SubmitPayment> Members

        public void Consume(SubmitPayment message)
        {
            Console.WriteLine("Cashier: got payment");
            bus.Publish(new PaymentComplete
            {
                CorrelationId = Id
            });
            IsCompleted = true;
        }

        #endregion
    }
}