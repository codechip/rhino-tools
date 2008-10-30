using System;
using Interfaces;

namespace CustomerCareModule
{
    public class Lead
    {
        public virtual long Id { get; set; }
        public virtual ICustomer Customer { get; set; }
        public virtual string Note { get; set; }
    }
}