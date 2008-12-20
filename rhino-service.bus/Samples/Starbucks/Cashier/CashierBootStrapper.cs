using Rhino.ServiceBus.Hosting;

namespace Starbucks.Cashier
{
    public class CashierBootStrapper : AbstractBootStrapper
    {
        protected override bool IsTypeAcceptableForThisBootStrapper(System.Type t)
        {
            return t.Namespace == "Starbucks.Cashier";
        }
    }
}