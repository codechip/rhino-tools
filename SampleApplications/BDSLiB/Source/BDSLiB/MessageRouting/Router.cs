namespace BDSLiB.MessageRouting
{
    using System.Text;
    using Boo.Lang;
    using BDSLiB.Properties;
    using DSL;
    using Rhino.DSL;

    /// <summary>
    /// This is a static gateway that dispatch messages using DSL
    /// to their appropriate handlers in the 
    /// </summary>
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
            RoutingBase[] routings = dslFactory.CreateAll<RoutingBase>(
                Settings.Default.RoutingScriptsDirectory
                );
            foreach (RoutingBase routing in routings)
            {
                routing.Initialize(msg);
                routing.Route();
            }
            if(messages.Length==0)
            {
                return "nothing can handle this message";
            }
            return messages.ToString();
        }
    }
}