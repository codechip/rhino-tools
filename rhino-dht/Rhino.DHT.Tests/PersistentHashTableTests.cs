using System.IO;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class PersistentHashTableTests
    {
        private const string testDatabase = "test.esent";

        public PersistentHashTableTests()
        {
            File.Delete(testDatabase);
        }

        [Fact]
        public void Can_save_and_load_item_from_cache()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("test", new int[0], new byte[] { 1 });
                    var values = actions.Get("test");
                    Assert.Equal(1, values[0].Version);
                    Assert.Equal(new byte[] { 1 }, values[0].Data);
                });
            }
        }

        [Fact]
        public void Can_get_item_in_specific_version()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    var version1 = actions.Put("test", new int[0], new byte[] { 1 });
                    actions.Put("test", new int[0], new byte[] { 2 });
                    var value = actions.Get("test", version1);
                    Assert.Equal(new byte[]{1}, value.Data);
                });
            }
        }

        [Fact]
        public void After_resolving_conflict_will_remove_old_version_of_data()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    var version1 = actions.Put("test", new int[0], new byte[] { 1 });
                    actions.Put("test", new int[0], new byte[] { 2 });
                    var value = actions.Get("test", version1);
                    Assert.Equal(new byte[] { 1 }, value.Data);

                    actions.Put("test", new[] {1, 2}, new byte[] {3});
                    Assert.Null(actions.Get("test", version1));
                });
            }
        }

        [Fact]
        public void Save_several_items_for_same_version_will_save_all_of_them()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("test", new int[0], new byte[] { 1 });
                    actions.Put("test", new int[0], new byte[] { 2 });
                    var values = actions.Get("test");
                    Assert.Equal(2, values.Length);

                    Assert.Equal(1, values[0].Version);
                    Assert.Equal(new byte[] { 1 }, values[0].Data);

                    Assert.Equal(2, values[1].Version);
                    Assert.Equal(new byte[] { 2 }, values[1].Data);
                });
            }
        }

        [Fact]
        public void Can_resolve_conflict()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("test", new int[0], new byte[] { 1 });
                    actions.Put("test", new int[0], new byte[] { 2 });
                    var values = actions.Get("test");
                    Assert.Equal(2, values.Length);
                    actions.Put("test", new[] {1, 2}, new byte[] {3});

                    values = actions.Get("test");
                    Assert.Equal(1, values.Length);
                    Assert.Equal(new byte[] {3}, values[0].Data);
                    Assert.Equal(3, values[0].Version);
                });
            }
        }

        [Fact]
        public void Cannot_query_with_partial_item_key()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 });
                    var values = actions.Get("abc10");
                    Assert.Equal(0, values.Length);

                    values = actions.Get("abc1");
                    Assert.NotEqual(0, values.Length);
                });
            }
        }


    }
}
