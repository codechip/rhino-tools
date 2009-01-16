using System.Net;
using System.Web;
using Rhino.Cache.Abstractions;

namespace Rhino.Cache.Handlers
{
    public class DelHandler : AbstractHandler
    {
        public DelHandler(IHttpContext context, PersistentCache persistentCache) : base(context, persistentCache)
        {
        }

        public override void Execute()
        {
            var key = Request.Path;
            InMemoryCache.Remove(key);
            PersistentCache.Remove(key);
            Response.StatusCode = 205;
            Response.StatusDescription = "ResetContent";
            Response.Status = "ResetContent";
        }
    }
}