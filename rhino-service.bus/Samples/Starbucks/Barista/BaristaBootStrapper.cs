using Rhino.ServiceBus.Hosting;

namespace Starbucks.Barista
{
    public class BaristaBootStrapper : AbstractBootStrapper
    {
        protected override bool IsTypeAcceptableForThisBootStrapper(System.Type t)
        {
            return t.Namespace == "Starbucks.Barista";
        }
    }
}