using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Xunit;
using System.Linq;

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
        public void Id_of_table_is_persistent()
        {
            Guid id;
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                id = table.Id;
                Assert.NotEqual(Guid.Empty, id);
            }

            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                Assert.NotEqual(Guid.Empty, table.Id);
                Assert.Equal(id, table.Id);
            }
        }

        [Fact]
        public void Replication_destination_is_persistent()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();
                table.AddReplicationDestination("net.tcp://foo.bar");
                table.AddReplicationDestination("net.tcp://baz.ban");
            }

            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                Assert.Equal("net.tcp://foo.bar", table.ReplicationDestinations[0]);
                Assert.Equal("net.tcp://baz.ban", table.ReplicationDestinations[1]);
            }
        }

        [Fact]
        public void Will_record_addition_for_replication()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();
                table.AddReplicationDestination("dummy");
                table.Batch(actions =>
                {
                    actions.Put("test", new ValueVersion[0], new byte[] { 123, 12, 12 });

                    Api.TryMoveFirst(actions.Session, actions.ReplicationActions);
                    var columns = Api.GetColumnDictionary(actions.Session, actions.ReplicationActions);
                    var key = Api.RetrieveColumnAsString(actions.Session, actions.ReplicationActions, columns["key"],
                                                       Encoding.Unicode);

                    var replicationAction = (ReplicationAction)Api.RetrieveColumnAsInt32(actions.Session, actions.ReplicationActions, columns["action_type"]).Value;

                    Assert.Equal("test", key);
                    Assert.Equal(ReplicationAction.Added, replicationAction);
                });
            }
        }

        [Fact]
        public void Will_record_removal_for_replication()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();
                table.AddReplicationDestination("dummy");

                table.Batch(actions =>
                {
                    var result = actions.Put("test", new ValueVersion[0], new byte[] { 123, 12, 12 });
                    actions.Remove("test", new[] { result.Version });

                    Api.TryMoveFirst(actions.Session, actions.ReplicationActions);

                    //skip add item
                    Api.TryMoveNext(actions.Session, actions.ReplicationActions);
                    var columns = Api.GetColumnDictionary(actions.Session, actions.ReplicationActions);
                    var key = Api.RetrieveColumnAsString(actions.Session, actions.ReplicationActions, columns["key"],
                                                       Encoding.Unicode);

                    var replicationAction = (ReplicationAction)Api.RetrieveColumnAsInt32(actions.Session, actions.ReplicationActions, columns["action_type"]).Value;

                    Assert.Equal("test", key);
                    Assert.Equal(ReplicationAction.Removed, replicationAction);
                });
            }
        }

        [Fact]
        public void Can_remove_destination()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();
                table.AddReplicationDestination("net.tcp://foo.bar");
                table.AddReplicationDestination("net.tcp://baz.ban");
            }

            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();
                table.RemoveReplicationSource("net.tcp://baz.ban");

                Assert.Equal(1, table.ReplicationDestinations.Length);
            }

            //and now we test that this is persistent
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                Assert.Equal(1, table.ReplicationDestinations.Length);
            }
        }



        [Fact]
        public void Can_save_and_load_item_from_cache()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    var values = actions.Get("test");
                    Assert.Equal(1, values[0].Version.Version);
                    Assert.Equal(new byte[] { 1 }, values[0].Data);
                });
            }
        }

        [Fact]
        public void Can_get_hash_from_cache()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    var bytes = Encoding.UTF8.GetBytes("abcdefgiklmnqrstwxyz");
                    actions.Put("test", new ValueVersion[0], bytes);
                    var values = actions.Get("test");
                    Assert.Equal(
                        SHA256.Create().ComputeHash(bytes),
                        values[0].Sha256Hash
                        );
                });
            }
        }

        [Fact]
        public void Can_remove_item_from_table_when_there_is_more_than_single_item()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                ValueVersion versionOfA = null, versionOfC = null;

                table.Batch(actions =>
                {
                    versionOfA = actions.Put("a", new ValueVersion[0], new byte[] { 1 }).Version;
                    actions.Put("b", new ValueVersion[0], new byte[] { 1 });
                    versionOfC = actions.Put("c", new ValueVersion[0], new byte[] { 1 }).Version;
                    actions.Put("d", new ValueVersion[0], new byte[] { 1 });

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
                    var version1 = actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    actions.Put("test", new ValueVersion[0], new byte[] { 2 });
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
                    var version1 = actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    var version2 = actions.Put("test", new ValueVersion[0], new byte[] { 2 });
                    var value = actions.Get("test", version1.Version);
                    Assert.Equal(new byte[] { 1 }, value.Data);

                    actions.Put("test", new[]
                    {
                        version1.Version,
                        version2.Version
                    }, new byte[] { 3 });

                    actions.Commit();

                    Assert.Null(actions.Get("test", version1.Version));
                });
            }
        }

        [Fact]
        public void Can_use_optimistic_concurrency()
        {
            using (var table = new PersistentHashTable(testDatabase))
            {
                table.Initialize();

                table.Batch(actions =>
                {
                    actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    var put = actions.Put("test", new ValueVersion[0], new byte[] { 2 }, null, false);
                    Assert.True(put.ConflictExists);

                    actions.Commit();

                    Assert.Equal(1, actions.Get("test").Length);
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
                    actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    actions.Put("test", new ValueVersion[0], new byte[] { 2 });
                    var values = actions.Get("test");
                    Assert.Equal(2, values.Length);

                    Assert.Equal(1, values[0].Version.Version);
                    Assert.Equal(new byte[] { 1 }, values[0].Data);

                    Assert.Equal(2, values[1].Version.Version);
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
                    var version1 = actions.Put("test", new ValueVersion[0], new byte[] { 1 });
                    var version2 = actions.Put("test", new ValueVersion[0], new byte[] { 2 });
                    var values = actions.Get("test");
                    Assert.Equal(2, values.Length);
                    actions.Put("test", new[] { version1.Version, version2.Version }, new byte[] { 3 });

                    values = actions.Get("test");
                    Assert.Equal(1, values.Length);
                    Assert.Equal(new byte[] { 3 }, values[0].Data);
                    Assert.Equal(3, values[0].Version.Version);
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
                    actions.Put("abc1", new ValueVersion[0], new byte[] { 1 });
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
                    actions.Put("abc1", new ValueVersion[0], new byte[] { 1 }, DateTime.Now.AddYears(1), true);

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
                    var version1 = actions.Put("abc1", new ValueVersion[0], new byte[] { 1 });

                    actions.Put("abc1", new[] { version1.Version }, new byte[] { 1 });
                    actions.Put("abc1", new[] { version1.Version }, new byte[] { 1 });
                    actions.Put("abc1", new[]
                    {
                        new ValueVersion
                        {
                            InstanceId = version1.Version.InstanceId,
                            Version = 3
                        },
                    }, new byte[] { 1 });

                    var values = actions.Get("abc1");
                    Assert.Equal(3, values.Length);

                    Assert.Equal(new[] { 1 }, values[0].ParentVersions.Select(x => x.Version).ToArray());
                    Assert.Equal(new[] { 1 }, values[1].ParentVersions.Select(x => x.Version).ToArray());
                    Assert.Equal(new[] { 3 }, values[2].ParentVersions.Select(x => x.Version).ToArray());
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
                    actions.Put("abc1", new ValueVersion[0], new byte[] { 1 },
                        DateTime.Now.AddYears(-1), true);
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
                    actions.Put("abc1", new ValueVersion[0], new byte[] { 1 },
                        DateTime.Now, true);

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
                    actions.Put("abc1", new ValueVersion[0], new byte[] { 1 });


                    table.Batch(actions2 =>
                    {
                        actions2.Put("dve", new ValueVersion[0], new byte[] { 3 });
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
