using Interfaces;

namespace HelpDeskPackage
{
    public class HelpDeskCustomer : ICustomer
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Phone { get; set; }
    }
}
