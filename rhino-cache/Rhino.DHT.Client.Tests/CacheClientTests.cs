using System;
using System.IO;
using System.Net;
using Xunit;

namespace Rhino.Cache.Client.Tests
{
    public class CacheClientTests : IDisposable
    {
        private readonly HttpListener[] listeners;
        private readonly ICache cache;

        public CacheClientTests()
        {
            RhinoCacheHandler.FileName = "test.esent";
            File.Delete(RhinoCacheHandler.FileName);

            listeners = new[]
            {
                GetListener("http://localhost:6212/"),
                GetListener("http://localhost:6213/"),
            };

            cache = new Cache(
                "http://localhost:6212/",
                "http://localhost:6213/"
                );

            foreach (var listener in listeners)
            {
                listener.Start();
            }
        }

        private static HttpListener GetListener(string prefix)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            listener.BeginGetContext(HandleRequest, listener);
            return listener;
        }

        private static void HandleRequest(IAsyncResult ar)
        {
            try
            {
                var listener = (HttpListener) ar.AsyncState;
                var context = listener.EndGetContext(ar);
                new RhinoCacheHandler().ProcessRequest(context);
                listener.BeginGetContext(HandleRequest, listener);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        [Fact]
        public void Can_add_item()
        {
            cache.Put("test","a", new byte[]{1,2,3});

            CachedItem item;
            cache.Get("test", out item);

            Assert.Equal("a", item.Type);
            Assert.Equal(new byte[] {1, 2, 3}, item.Data);
        }

        [Fact]
        public void Can_remove_item()
        {
            cache.Put("test", "a", new byte[] { 1, 2, 3 });

            CachedItem item;
            cache.Get("test", out item);

            Assert.Equal("a", item.Type);
            Assert.Equal(new byte[] { 1, 2, 3 }, item.Data);

            cache.Delete("test");

            var found = cache.Get("test", out item);
            Assert.False(found);
        }

        [Fact]
        public void Can_add_items_to_different_endpoints()
        {
            Assert.NotEqual(
                "test1".GetHashCode() % 2, 
                "test76".GetHashCode() % 2);

            cache.Put("test1", "a", new byte[] { 12});
            cache.Put("test76", "b", new byte[] { 13 });

            CachedItem item;
            cache.Get("test1", out item);

            Assert.Equal("a", item.Type);
            Assert.Equal(new byte[] { 12}, item.Data);

            cache.Get("test76", out item);

            Assert.Equal("b", item.Type);
            Assert.Equal(new byte[] { 13 }, item.Data);
        }

        public void Dispose()
        {
            foreach (var listener in listeners)
            {
                listener.Stop();
                listener.Close();
            }
            RhinoCacheHandler.Reset();
        }
    }
}
