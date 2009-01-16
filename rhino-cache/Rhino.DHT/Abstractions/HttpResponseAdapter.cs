using System.IO;
using System.Web;

namespace Rhino.Cache.Abstractions
{
    public class HttpResponseAdapter : IHttpResponse
    {
        private readonly HttpContext context;

        public HttpResponseAdapter(HttpContext context)
        {
            this.context = context;
        }

        public string StatusDescription
        {
            get { return context.Response.StatusDescription; }
            set { context.Response.StatusDescription = value;}
        }

        public int StatusCode
        {
            get { return context.Response.StatusCode; }
            set { context.Response.StatusCode = value; }
        }

        public string Status
        {
            get { return context.Response.Status; }
            set { context.Response.Status = value;}
        }

        public string ContentType
        {
            get { return context.Response.ContentType; }
            set { context.Response.ContentType = value;}
        }

        public Stream OutputStream
        {
            get { return context.Response.OutputStream; }
        }

        public void AddHeader(string name, string value)
        {
            context.Response.AddHeader(name, value);
        }

        public void End()
        {
            context.Response.End();
        }
    }
}