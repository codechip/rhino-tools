using System.Web;
using Rhino.DHT.Abstractions;
using Rhino.DHT.Abstractions;

namespace Rhino.DHT.Handlers
{
    public abstract class AbstractHandler
    {
        private readonly IHttpContext context;
        protected readonly PersistentCache PersistentCache;

        protected AbstractHandler(IHttpContext context, PersistentCache persistentCache)
        {
            this.context = context;
            PersistentCache = persistentCache;
        }

        public abstract void Execute();

        public IHttpRequest Request
        {
            get { return context.Request; }
        }

        public IHttpResponse Response
        {
            get { return context.Response; }
        }

        public System.Web.Caching.Cache InMemoryCache
        {
            get { return context.Cache; }
        }
    }
}