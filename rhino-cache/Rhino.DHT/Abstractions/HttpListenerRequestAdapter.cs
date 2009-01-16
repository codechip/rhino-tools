using System.IO;
using System.Net;

namespace Rhino.DHT.Abstractions
{
    public class HttpListenerRequestAdapter : IHttpRequest
    {
        private readonly HttpListenerContext context;

        public HttpListenerRequestAdapter(HttpListenerContext context)
        {
            this.context = context;
        }

        #region IHttpRequest Members

        public string Path
        {
            get { return context.Request.Url.PathAndQuery; }
        }

        public string ContentType
        {
            get { return context.Request.ContentType; }
        }

        public Stream InputStream
        {
            get { return context.Request.InputStream; }
        }

        public string HttpMethod
        {
            get { return context.Request.HttpMethod; }
        }

        #endregion
    }
}