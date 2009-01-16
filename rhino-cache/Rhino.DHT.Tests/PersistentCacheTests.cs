using System;
using System.IO;
using Rhino.DHT.Handlers;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class PersistentCacheTests : IDisposable
    {
        private readonly PersistentCache cache;

        public PersistentCacheTests()
        {
            File.Delete("test.esent");
            cache = new PersistentCache("test.esent");
        }

        [Fact]
        public void Can_save_and_load_item_to_cache()
        {
            var buffer = new byte[]{12,34,43};
            cache.Put(new CacheItem
            {
                Data = buffer,
                Key = "foo",
                Type = "bar"
            });

            var operation = cache.Get("foo");
            Assert.Equal("bar", operation.Type);
            Assert.Equal(buffer, operation.Data);
        }

        [Fact]
        public void Can_overwrite()
        {
            var buffer = new byte[] { 12, 34, 43 };
            cache.Put(new CacheItem
            {
                Data = buffer,
                Key = "foo",
                Type = "ba2r"
            });

            var operation = cache.Get("foo");
            Assert.Equal("ba2r", operation.Type);
        }


        [Fact]
        public void Will_return_null_if_cannot_find_item()
        {
            Assert.Null(cache.Get("foo"));
        }

        [Fact]
        public void Will_record_removal()
        {
            var buffer = new byte[] { 12, 34, 43 };
            cache.Put(new CacheItem
            {
                Data = buffer,
                Key = "foo",
                Type = "bar"
            });

            cache.Put(new CacheItem
            {
                Data = buffer,
                Key = "foo2",
                Type = "bar"
            });

            cache.Remove("foo");

            Assert.Null(cache.Get("foo"));

            Assert.NotNull(cache.Get("foo2"));
        }

        public void Dispose()
        {
            cache.Dispose();
        }
    }
}