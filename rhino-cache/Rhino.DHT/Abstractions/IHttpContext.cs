namespace Rhino.DHT.Abstractions
{
    public interface IHttpContext
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
    }
}