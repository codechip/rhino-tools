using System.IO;
using System.Web;

namespace Rhino.DHT.Abstractions
{
    public class HttpRequestAdapter : IHttpRequest
    {
        private readonly HttpContext context;

        public HttpRequestAdapter(HttpContext context)
        {
            this.context = context;
        }

        public string Path
        {
            get { return context.Request.Url.PathAndQuery; }
        }

        public string ContentType
        {
            get { return context.Request.ContentType;}
        }

        public Stream InputStream
        {
            get { return context.Request.InputStream; }
        }

        public string HttpMethod
        {
            get { return context.Request.HttpMethod; }
        }
    }
}