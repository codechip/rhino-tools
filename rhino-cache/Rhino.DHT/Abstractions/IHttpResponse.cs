using System.IO;

namespace Rhino.DHT.Abstractions
{
    public interface IHttpResponse
    {
        string StatusDescription { get; set; }
        int StatusCode { get; set; }
        string Status { get; set; }
        string ContentType { get; set; }
        Stream OutputStream { get; }
        void AddHeader(string name, string value);
        void End();
    }
}