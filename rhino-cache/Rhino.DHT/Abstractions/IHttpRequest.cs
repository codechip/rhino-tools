using System.IO;

namespace Rhino.DHT.Abstractions
{
    public interface IHttpRequest
    {
        string Path { get; }
        string ContentType { get; }
        Stream InputStream { get; }
        string HttpMethod { get; }
    }
}