using System.Web;
using Rhino.DHT.Abstractions;
using Rhino.DHT.Abstractions;

namespace Rhino.DHT.Handlers
{
    public class GetHandler : AbstractHandler
    {
        public GetHandler(IHttpContext context, PersistentCache persistentCache)
            : base(context, persistentCache)
        {
        }

        public override void Execute()
        {
            var key = Request.Path;
            if(key.Length>255)
            {
                Response.StatusDescription = "path must be less than 256 charaters";
                Response.StatusCode = 500;
                Response.Status = "Bad Request";
                return;
            }
            var cache = InMemoryCache[key] as CacheItem ?? 
                PersistentCache.Get(key);

            if(cache==null)
            {
                Response.Status = "Not Found";
                Response.StatusCode = 404;
            }
            else
            {
                Response.ContentType = cache.Type;
                Response.AddHeader("rhino-cache-key", cache.Key);
                Response.OutputStream.Write(cache.Data, 0, cache.Data.Length);
            }
        }
    }
}