using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
    public class PersistentHashTableActions : IDisposable
    {
        private readonly string database;
        private readonly Session session;
        private readonly Transaction transaction;
        private readonly Table keys;
        private readonly Table data;
        private readonly JET_DBID dbid;
        private readonly Dictionary<string, JET_COLUMNID> keysColumns;
        private readonly Dictionary<string, JET_COLUMNID> dataColumns;
        private readonly Cache cache;
        private readonly List<Action> commitSyncronization = new List<Action>();

        public PersistentHashTableActions(Instance instance, string database, Cache cache)
        {
            this.database = database;
            this.cache = cache;
            session = new Session(instance);

            Api.JetAttachDatabase(session, database, AttachDatabaseGrbit.None);
            transaction = new Transaction(session);
            Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.None);
            keys = new Table(session, dbid, "keys", OpenTableGrbit.None);
            data = new Table(session, dbid, "data", OpenTableGrbit.None);
            keysColumns = Api.GetColumnDictionary(session, keys);
            dataColumns = Api.GetColumnDictionary(session, data);
        }

        public int Put(string key, int[] parentVersions, byte[] bytes)
        {
            // always remove the active versions from the cache
            commitSyncronization.Add(() => cache.Remove(GetKey(key)));
            if (DoesAllVersionsMatch(key, parentVersions))
            {
                // we only remove existing versions from the 
                // cache if we delete them from the database
                foreach (var parentVersion in parentVersions)
                {
                    var copy = parentVersion;
                    commitSyncronization.Add(() => cache.Remove(GetKey(key, copy)));
                }
                DeleteInactiveVersions(key, parentVersions);
            }

            var bookmark = new byte[Api.BookmarkMost];
            int bookmarkSize;
            using (var update = new Update(session, keys, JET_prep.Insert))
            {
                Api.SetColumn(session, keys, keysColumns["key"], key, Encoding.Unicode);

                update.Save(bookmark, bookmark.Length, out bookmarkSize);
            }

            Api.JetGotoBookmark(session, keys, bookmark, bookmarkSize);
            var version = Api.RetrieveColumnAsInt32(session, keys, keysColumns["version"]);

            using (var update = new Update(session, data, JET_prep.Insert))
            {
                Api.SetColumn(session, data, dataColumns["key"], key, Encoding.Unicode);
                Api.SetColumn(session, data, dataColumns["version"], version.Value);
                Api.SetColumn(session, data, dataColumns["data"], bytes);

                update.Save();
            }

            return version.Value;
        }

        private bool DoesAllVersionsMatch(string key, IEnumerable<int> parentVersions)
        {
            var activeVersions = GatherActiveVersion(key)
                .OrderBy(x => x);

            return parentVersions
                .OrderBy(x => x)
                .SequenceEqual(activeVersions.OrderBy(x => x));
        }

        public Value[] Get(string key)
        {
            var values = new List<Value>();
            var activeVersions = GatherActiveVersion(key);

            bool foundAllInCache = true;
            foreach (var activeVersion in activeVersions)
            {
                var cachedValue = cache[GetKey(key, activeVersion)];
                if (cachedValue == null ||
                    cachedValue == DBNull.Value)
                {
                    values.Clear();
                    foundAllInCache = false;
                    break;
                }
                values.Add((Value)cachedValue);
            }
            if (foundAllInCache)
                return values.ToArray();

            ApplyToKeyAndActiveVersions(data, activeVersions, key, version =>
                values.Add(new Value
                {
                    Version = version,
                    Key = key,
                    Data = Api.RetrieveColumn(session, data, dataColumns["data"])
                })
            );

            commitSyncronization.Add(delegate
            {
                foreach (var value in values)
                {
                    cache[GetKey(value.Key, value.Version)] = value;
                }
                cache[GetKey(key)] = activeVersions;
            });

            return values.ToArray();
        }

        public Value Get(string key, int specifiedVersion)
        {
            var cachedValue = cache[GetKey(key, specifiedVersion)];
            if (cachedValue != null && 
                cachedValue != DBNull.Value)
                return (Value)cachedValue;

            Value val = null;
            ApplyToKeyAndActiveVersions(data, new[] { specifiedVersion }, key, version =>
            {
                val = new Value
                {
                    Key = key,
                    Data = Api.RetrieveColumn(session, data, dataColumns["data"]),
                    Version = specifiedVersion
                };
            });
            cache[GetKey(key, specifiedVersion)] = (object)val ?? DBNull.Value;
            return val;
        }

        private string GetKey(string key, int version)
        {
            return GetKey(key) + "#" + version;
        }

        private string GetKey(string key)
        {
            return "rhino.dht [" + database + "]: " + key;
        }

        public void Commit()
        {
            transaction.Commit(CommitTransactionGrbit.None);
            foreach (var action in commitSyncronization)
            {
                action();
            }
        }

        private void DeleteInactiveVersions(string key, IEnumerable<int> versions)
        {
            ApplyToKeyAndActiveVersions(keys, versions, key,
                version => Api.JetDelete(session, keys));

            ApplyToKeyAndActiveVersions(data, versions, key, version =>
                Api.JetDelete(session, data));
        }

        private void ApplyToKeyAndActiveVersions(Table table, IEnumerable<int> versions, string key, Action<int> action)
        {
            Api.JetSetCurrentIndex(session, table, "pk");
            foreach (var version in versions)
            {
                Api.MakeKey(session, table, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
                Api.MakeKey(session, table, version, MakeKeyGrbit.None);

                if (Api.TrySeek(session, table, SeekGrbit.SeekEQ) == false)
                    continue;

                action(version);
            }
        }

        private int[] GatherActiveVersion(string key)
        {
            var cachedActiveVersions = cache[GetKey(key)];
            if (cachedActiveVersions != null)
                return (int[])cachedActiveVersions;

            Api.JetSetCurrentIndex(session, keys, "by_key");
            Api.MakeKey(session, keys, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
            var exists = Api.TrySeek(session, keys, SeekGrbit.SeekEQ);
            if (exists == false)
                return new int[0];

            Api.MakeKey(session, keys, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
            Api.JetSetIndexRange(session, keys, 
                SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive);

            var ids = new List<int>();
            var columns = Api.GetColumnDictionary(session, keys);
            do
            {
                var version = Api.RetrieveColumnAsInt32(session, keys, columns["version"]);
                ids.Add(version.Value);
            } while (Api.TryMoveNext(session, keys));
            return ids.ToArray();
        }

        public void Dispose()
        {
            if (keys != null)
                keys.Dispose();
            if (data != null)
                data.Dispose();

            if (Equals(dbid, JET_DBID.Nil) == false)
                Api.JetCloseDatabase(session, dbid, CloseDatabaseGrbit.None);

            if (transaction != null)
                transaction.Dispose();

            Api.JetDetachDatabase(session, database);

            if (session != null)
                session.Dispose();
        }

        public bool Remove(string key, int[] parentVersions)
        {
            var doesAllVersionsMatch = DoesAllVersionsMatch(key, parentVersions);
            if (doesAllVersionsMatch)
            {
                DeleteInactiveVersions(key, parentVersions);

                foreach (var version in parentVersions)
                {
                    var copy = version;
                    commitSyncronization.Add(() => cache.Remove(GetKey(key, copy)));
                }
                commitSyncronization.Add(() => cache.Remove(GetKey(key)));
            }
            return doesAllVersionsMatch;
        }
    }
}