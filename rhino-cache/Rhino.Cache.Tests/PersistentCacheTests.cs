using System;
using System.IO;
using Rhino.Cache.Handlers;
using Xunit;

namespace Rhino.Cache.Tests
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
            cache.Put(new AddToCache
            {
                Data = buffer,
                Key = "foo",
                Type = "bar"
            });

            var operation = (AddToCache)cache.Get("foo");
            Assert.Equal("bar", operation.Type);
            Assert.Equal(buffer, operation.Data);
        }

        [Fact]
        public void Can_overwrite()
        {
            var buffer = new byte[] { 12, 34, 43 };
            cache.Put(new AddToCache
            {
                Data = buffer,
                Key = "foo",
                Type = "ba2r"
            });

            var operation = (AddToCache)cache.Get("foo");
            Assert.Equal("ba2r", operation.Type);
        }


        [Fact]
        public void Will_return_null_if_cannot_find_item()
        {
            Assert.Null(cache.Get("foo"));
        }

        [Fact]
        public void Will_expire_item_deletion()
        {
            var buffer = new byte[] { 12, 34, 43 };
            cache.Put(new AddToCache
            {
                Data = buffer,
                Key = "foo",
                Type = "bar"
            });

            cache.Put(new AddToCache
            {
                Data = buffer,
                Key = "foo2",
                Type = "bar"
            });

            cache.NumberOfDeletedRecordToKeepAround = 1;

            cache.Put(new RemoveFromCache()
            {
                Key = "foo",
            });

            Assert.IsType<RemoveFromCache>(cache.Get("foo"));

            cache.Put(new RemoveFromCache()
            {
                Key = "foo2",
            });

            Assert.Null(cache.Get("foo"));

            Assert.IsType<RemoveFromCache>(cache.Get("foo2"));

        }

        [Fact]
        public void Will_record_removal()
        {
            var buffer = new byte[] { 12, 34, 43 };
            cache.Put(new AddToCache
            {
                Data = buffer,
                Key = "foo",
                Type = "bar"
            });

            cache.Put(new RemoveFromCache()
            {
                Key = "foo",
            });

            Assert.IsType<RemoveFromCache>(cache.Get("foo"));
        }

        public void Dispose()
        {
            cache.Dispose();
        }
    }
}