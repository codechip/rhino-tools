using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollSystem.Common
{
    public interface IPaymentService
    {
        void Pay();
    }

    public class PaymentService : IPaymentService
    {
        IOverTimeCalculator calculator;

        public PaymentService(IOverTimeCalculator calculator)
        {
            this.calculator = calculator;
        }

        public void Pay()
        {
        }
    }
}
