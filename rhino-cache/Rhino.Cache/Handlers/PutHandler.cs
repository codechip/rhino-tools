using System.IO;
using System.Web;
using Rhino.Cache.Abstractions;

namespace Rhino.Cache.Handlers
{
    public class PutHandler : AbstractHandler
    {
        public PutHandler(IHttpContext context, PersistentCache persistentCache)
            : base(context, persistentCache)
        {
        }

        public override void Execute()
        {
            using (var ms = new MemoryStream())
            {
                var buffer = new byte[1024 * 1024];
                int read;
                do
                {
                    read = Request.InputStream.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, read);
                } while (read != 0);

                var key = Request.Path;
                var type = Request.ContentType ?? "binary/octet-stream";

                var operation = new CacheItem
                {
                    Key = key,
                    Type = type,
                    Data = ms.ToArray()
                };

                InMemoryCache[operation.Key] = operation;
                PersistentCache.Put(operation);

                Response.Status = "Accepted";
                Response.StatusCode = 202;

            }
        }
    }
}