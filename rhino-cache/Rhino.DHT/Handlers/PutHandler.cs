using System.IO;
using Rhino.DHT.Abstractions;

namespace Rhino.DHT.Handlers
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

                PersistentCache.Put(operation);

                Response.Status = "Accepted";
                Response.StatusCode = 202;

            }
        }
    }
}