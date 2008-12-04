using System;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Advance.IoC.GenericSpecialization.Validation
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For(typeof(IValidator<>))
                    .ImplementedBy(typeof(DefaultValidator<>)),
                Component.For<IValidator<Customer>>()
                    .ImplementedBy<CreditHigherThanDebitValidator>()
                );
            var customer = new Customer {Credit = 4, Debit = 31, Name = null};
            IValidator<Customer>[] validators = 
                container.ResolveAll<IValidator<Customer>>();
            foreach (var validator in validators)
            {
                foreach (var str in validator.Validate(customer))
                {
                    Console.WriteLine(str);
                }
            }
        }
    }
}
