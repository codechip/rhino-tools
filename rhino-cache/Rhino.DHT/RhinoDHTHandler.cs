using System.Net;
using System.Web;
using Rhino.DHT.Abstractions;
using Rhino.DHT.Handlers;

namespace Rhino.DHT
{
    public class RhinoDHTHandler : IHttpHandler
    {
        public static string Version =
            typeof(RhinoDHTHandler)
                .Assembly
                .GetName()
                .Version.ToString();

        private static readonly object persistentCacheLocker = new object();
        private static PersistentCache persistentCache;
        public static string FileName = "cache.esent";


        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpContextAdapter(context));
        }

        public void ProcessRequest(HttpListenerContext context)
        {
            ProcessRequest(new HttpListenerContextAdapter(context));
        }

        private static void ProcessRequest(IHttpContext context)
        {
            InitPersistentCacheIfNotStarted();

            context.Response.AddHeader("rhino-cache", Version);

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    new GetHandler(context, persistentCache).Execute();
                    break;
                case "DEL":
                    new DelHandler(context, persistentCache).Execute();
                    break;
                case "PUT":
                    new PutHandler(context, persistentCache).Execute();
                    break;
                default:
                    context.Response.Status = "Http method " + context.Request.HttpMethod +
                                                  " is not supported by Rhino.Cache";
                    context.Response.StatusCode = 400;
                    break;
            }

            context.Response.End();
        }

        public static void Reset()
        {
            lock(persistentCacheLocker)
            {
                if (persistentCache != null)
                    persistentCache.Dispose();
                persistentCache = null;
            }
        }

        private static void InitPersistentCacheIfNotStarted()
        {
            if (persistentCache != null)
                return;
            lock (persistentCacheLocker)
            {
                if (persistentCache != null)
                    return;
                var copy = new PersistentCache(FileName);
                persistentCache = copy;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}