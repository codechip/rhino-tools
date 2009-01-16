using System.Web;

namespace Rhino.Cache.Abstractions
{
    public class HttpContextAdapter : IHttpContext
    {
        private HttpContext context;

        public HttpContextAdapter(HttpContext context)
        {
            this.context = context;
        }

        public IHttpRequest Request
        {
            get { return new HttpRequestAdapter(context); }
        }

        public IHttpResponse Response
        {
            get { return new HttpResponseAdapter(context); }
        }

        public System.Web.Caching.Cache Cache
        {
            get { return context.Cache; }
        }
    }
}