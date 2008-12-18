using System;
using System.Threading;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Sagas;
using Starbucks.Messages;

namespace Starbucks.Barista
{
    public class BaristaSaga :
        InitiatedBy<PrepareDrink>,
        Orchestrates<PaymentComplete>
    {
        private readonly IServiceBus bus;
        private string drink;

        public BaristaSaga(IServiceBus bus)
        {
            this.bus = bus;
        }

        public bool DrinkIsReady { get; set; }

        public bool GotPayment { get; set; }

        #region InitiatedBy<PrepareDrink> Members

        public void Consume(PrepareDrink message)
        {
            drink = message.DrinkName;

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("preparing drink: " + drink);
                Thread.Sleep(500);
            }
            DrinkIsReady = true;
            SubmitOrderIfDone();
        }

        private void SubmitOrderIfDone()
        {
            if (GotPayment && DrinkIsReady)
            {
                bus.Publish(new DrinkReady
                {
                    CorrelationId = Id,
                    Drink = drink
                });
                IsCompleted = true;
            }
        }

        public Guid Id { get; set; }

        public bool IsCompleted { get; set; }

        #endregion

        #region Orchestrates<PaymentComplete> Members

        public void Consume(PaymentComplete message)
        {
            GotPayment = true;
            SubmitOrderIfDone();
        }

        #endregion
    }
}