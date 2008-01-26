namespace Chapter5
{
    using System.Text;
    using Boo.Lang;
    using Chapter5.MessageRouting.DSL;
    using MessageRouting;
    using Properties;
    using Rhino.DSL;

    public static class Router
    {
        private static readonly DslFactory dslFactory;

        static Router()
        {
            dslFactory = new DslFactory();
            dslFactory.Register<RoutingBase>(new RoutingDslEngine());
        }

        public static string Route(IQuackFu msg)
        {
            StringBuilder messages = new StringBuilder();
            RoutingBase[] routings = dslFactory.CreateAll<RoutingBase>(Settings.Default.RoutingScriptsDirectory);
            foreach (RoutingBase routing in routings)
            {
                routing.Initialize(msg);
                routing.Route();
                if(routing.Result!=null)
                {
                    messages.AppendLine(routing.GetType().Name)
                        .AppendLine(routing.Result);
                }
            }
            if(messages.Length==0)
            {
                return "nothing can handle this message";
            }
            return messages.ToString();
        }
    }
}
