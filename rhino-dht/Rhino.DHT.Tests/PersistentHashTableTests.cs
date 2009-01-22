using System;
using System.IO;
using Microsoft.Isam.Esent.Interop;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class PersistentHashTableTests
    {
        private const string testDatabase = "test.esent";

        public PersistentHashTableTests()
        {
            if (Directory.Exists(testDatabase))
                Directory.Delete(testDatabase, true);
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
        public void Can_remove_item_from_table_when_there_is_more_than_single_item()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                int versionOfA = 0, versionOfC = 0;

                table.Batch(actions =>
                {
                    versionOfA = actions.Put("a", new int[0], new byte[] { 1 }).Version;
                    actions.Put("b", new int[0], new byte[] { 1 });
                    versionOfC = actions.Put("c", new int[0], new byte[] { 1 }).Version;
                    actions.Put("d", new int[0], new byte[] { 1 });

                    actions.Commit();
                });

                table.Batch(actions =>
                {
                    var removed = actions.Remove("a", new[] { versionOfA });
                    Assert.True(removed);
                    removed = actions.Remove("c", new[] { versionOfC });
                    Assert.True(removed);
                    actions.Commit();
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
                    var value = actions.Get("test", version1.Version);
                    Assert.Equal(new byte[] { 1 }, value.Data);
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
                    var value = actions.Get("test", version1.Version);
                    Assert.Equal(new byte[] { 1 }, value.Data);

                    actions.Put("test", new[] { 1, 2 }, new byte[] { 3 });

                    actions.Commit();

                    Assert.Null(actions.Get("test", version1.Version));
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
                    actions.Put("test", new[] { 1, 2 }, new byte[] { 3 });

                    values = actions.Get("test");
                    Assert.Equal(1, values.Length);
                    Assert.Equal(new byte[] { 3 }, values[0].Data);
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

        [Fact]
        public void Can_query_for_item_with_expiry()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 }, DateTime.Now.AddYears(1));

                    var values = actions.Get("abc1");
                    Assert.NotEqual(0, values.Length);
                });
            }
        }

        [Fact]
        public void Can_query_for_item_history()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 });

                    actions.Put("abc1", new[] { 1 }, new byte[] { 1 });
                    actions.Put("abc1", new[] { 1 }, new byte[] { 1 });
                    actions.Put("abc1", new[] { 3 }, new byte[] { 1 });

                    var values = actions.Get("abc1");
                    Assert.Equal(3, values.Length);

                    Assert.Equal(new[] { 1 }, values[0].ParentVersions);
                    Assert.Equal(new[] { 1 }, values[1].ParentVersions);
                    Assert.Equal(new[] { 3 }, values[2].ParentVersions);
                });
            }
        }

        [Fact]
        public void After_item_expires_it_cannot_be_retrieved()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 },
                        DateTime.Now.AddYears(-1));
                    var values = actions.Get("abc1");

                    Assert.Equal(0, values.Length);
                });
            }
        }

        [Fact]
        public void After_item_expires_it_will_be_removed_on_next_commit()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 },
                        DateTime.Now);

                    actions.Commit();
                });

                table.Batch(actions => actions.Commit());

                int numRecords = -1;
                table.Batch(actions =>
                {
                    Api.JetSetCurrentIndex(actions.Session, actions.Keys, null);//primary
                    Api.JetIndexRecordCount(actions.Session, actions.Keys, out numRecords, 0);
                });

                Assert.Equal(0, numRecords);
            }
        }

        [Fact]
        public void Interleaved()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("abc1", new int[0], new byte[] { 1 });


                    table.Batch(actions2=>
                    {
                        actions2.Put("dve", new int[0], new byte[] {3});
                        actions2.Commit();
                    });

                    actions.Commit();
                });

                table.Batch(actions =>
                {
                    Assert.NotEmpty(actions.Get("abc1"));
                    Assert.NotEmpty(actions.Get("dve"));
                });
            }
        }
    }
}
