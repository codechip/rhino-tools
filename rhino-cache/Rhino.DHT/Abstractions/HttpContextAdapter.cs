using System.Web;

namespace Rhino.DHT.Abstractions
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
    }
}