using System;
using Interfaces;

namespace AccountingPackage
{
    public class AccountingCustomer : ICustomer
    {
        public virtual string Name { get; set; }
        public virtual Guid Id { get; set; }
        public virtual decimal Debt { get; set; }
    }
}
