using Rhino.ServiceBus.Hosting;

namespace Starbucks.Customer
{
    public class CustomerBootStrapper : AbstractBootStrapper
    {
        protected override bool IsTypeAcceptableForThisBootStrapper(System.Type t)
        {
            return t.Namespace == "Starbucks.Customer";
        }
    }
}