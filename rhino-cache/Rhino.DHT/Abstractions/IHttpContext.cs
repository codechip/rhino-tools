namespace Rhino.Cache.Abstractions
{
    public interface IHttpContext
    {
        IHttpRequest Request { get; }
        IHttpResponse Response { get; }
        System.Web.Caching.Cache Cache { get; }
    }
}