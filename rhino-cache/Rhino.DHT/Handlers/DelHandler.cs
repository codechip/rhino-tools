using Rhino.DHT.Abstractions;

namespace Rhino.DHT.Handlers
{
    public class DelHandler : AbstractHandler
    {
        public DelHandler(IHttpContext context, PersistentCache persistentCache) : base(context, persistentCache)
        {
        }

        public override void Execute()
        {
            var key = Request.Path;
            PersistentCache.Remove(key);
            Response.StatusCode = 205;
            Response.StatusDescription = "ResetContent";
            Response.Status = "ResetContent";
        }
    }
}