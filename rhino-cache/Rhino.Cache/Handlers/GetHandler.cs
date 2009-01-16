using System.Web;
using Rhino.Cache.Abstractions;

namespace Rhino.Cache.Handlers
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
            var cache = InMemoryCache[key] as CacheOperation;
            if (cache == null)
            {
                cache = PersistentCache.Get(key);
            }

            if(cache==null)
            {
                Response.Status = "Not Found";
                Response.StatusCode = 404;
            }
            else if(cache is RemoveFromCache)
            {
                Response.Status = "Gone";
                Response.StatusCode = 410;
                return;
            }
            else
            {
                var item = (AddToCache) cache;
                Response.ContentType = item.Type;
                Response.AddHeader("rhino-cache-key", item.Key);
                Response.OutputStream.Write(item.Data, 0, item.Data.Length);
            }
        }
    }
}