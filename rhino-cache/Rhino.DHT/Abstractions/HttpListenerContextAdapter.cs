using System.Net;
using System.Web;

namespace Rhino.DHT.Abstractions
{
    public class HttpListenerContextAdapter : IHttpContext
    {
        private readonly HttpListenerContext context;

        public HttpListenerContextAdapter(HttpListenerContext context)
        {
            this.context = context;
        }

        public IHttpRequest Request
        {
            get { return new HttpListenerRequestAdapter(context); }
        }

        public IHttpResponse Response
        {
            get { return new HttpListenerResponseAdapter(context); }
        }
    }
}