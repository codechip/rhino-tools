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
            var cache = new RemoveFromCache {Key = key};
            InMemoryCache[key] = cache;
            PersistentCache.Put(cache);
            Response.StatusCode = 205;
            Response.StatusDescription = "ResetContent";
            Response.Status = "ResetContent";
        }
    }
}