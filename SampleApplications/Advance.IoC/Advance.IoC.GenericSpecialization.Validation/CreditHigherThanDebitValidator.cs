namespace Advance.IoC.GenericSpecialization.Validation
{
    public class CreditHigherThanDebitValidator : IValidator<Customer>
    {
        public string[] Validate(Customer instnace)
        {
            if (instnace.Credit < instnace.Debit)
            {
                return new [] { "Debit higher than credit" };
            }
            return new string[0];
        }
    }
}