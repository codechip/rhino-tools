using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly Guid instanceId;
        private readonly JET_DBID dbid;
        private readonly Dictionary<string, JET_COLUMNID> keysColumns;
        private readonly Dictionary<string, JET_COLUMNID> dataColumns;
        private readonly Cache cache;
        private readonly List<Action> commitSyncronization = new List<Action>();

        public JET_DBID DatabaseId
        {
            get { return dbid; }
        }

        public Session Session
        {
            get { return session; }
        }

        public Transaction Transaction
        {
            get { return transaction; }
        }

        public Table Keys
        {
            get { return keys; }
        }

        public Table Data
        {
            get { return data; }
        }

        public Dictionary<string, JET_COLUMNID> KeysColumns
        {
            get { return keysColumns; }
        }

        public Dictionary<string, JET_COLUMNID> DataColumns
        {
            get { return dataColumns; }
        }

        public PersistentHashTableActions(Instance instance, string database, Cache cache, Guid instanceId)
        {
            this.database = database;
            this.cache = cache;
            this.instanceId = instanceId;
            session = new Session(instance);

            transaction = new Transaction(session);
            Api.JetOpenDatabase(session, database, null, out dbid, OpenDatabaseGrbit.None);
            keys = new Table(session, dbid, "keys", OpenTableGrbit.None);
            data = new Table(session, dbid, "data", OpenTableGrbit.None);
            keysColumns = Api.GetColumnDictionary(session, keys);
            dataColumns = Api.GetColumnDictionary(session, data);
        }

        public PutResult Put(string key, ValueVersion[] parentVersions, byte[] bytes)
        {
            return Put(key, parentVersions, bytes, null, true);
        }

        public PutResult Put(string key, ValueVersion[] parentVersions, byte[] bytes, DateTime? expiresAt, bool allowConflict)
        {
            var doesAllVersionsMatch = DoesAllVersionsMatch(key, parentVersions);

            if (doesAllVersionsMatch == false && allowConflict == false)
            {
                return new PutResult
                {
                    ConflictExists = true,
                    Version = null
                };
            }

            // always remove the active versions from the cache
            commitSyncronization.Add(() => cache.Remove(GetKey(key)));
            if (doesAllVersionsMatch)
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
                Api.SetColumn(session, keys, keysColumns["version_instance_id"], instanceId.ToByteArray());

                if (expiresAt.HasValue)
                    Api.SetColumn(session, keys, keysColumns["expiresAt"], expiresAt.Value.ToOADate());

                update.Save(bookmark, bookmark.Length, out bookmarkSize);
            }

            Api.JetGotoBookmark(session, keys, bookmark, bookmarkSize);
            var version = Api.RetrieveColumnAsInt32(session, keys, keysColumns["version_number"]);

            using (var update = new Update(session, data, JET_prep.Insert))
            {
                Api.SetColumn(session, data, dataColumns["key"], key, Encoding.Unicode);
                Api.SetColumn(session, data, dataColumns["version_number"], version.Value);
                Api.SetColumn(session, data, dataColumns["version_instance_id"], instanceId.ToByteArray());
                Api.SetColumn(session, data, dataColumns["data"], bytes);
                using (var sha256 = SHA256.Create())
                {
                    Api.SetColumn(session, data, dataColumns["sha256_hash"], sha256.ComputeHash(bytes));
                }

                if (expiresAt.HasValue)
                    Api.SetColumn(session, data, dataColumns["expiresAt"], expiresAt.Value.ToOADate());

                using (var stream = new ColumnStream(session, data, dataColumns["parentVersions"]))
                {
                    foreach (var parentVersion in parentVersions)
                    {
                        var versionAsBytes = parentVersion.InstanceId.ToByteArray();
                        stream.Write(versionAsBytes, 0, versionAsBytes.Length);
                        versionAsBytes = BitConverter.GetBytes(parentVersion.Version);
                        stream.Write(versionAsBytes, 0, versionAsBytes.Length);
                        stream.Itag += 1;
                    }
                }

                update.Save();
            }

            return new PutResult
            {
                ConflictExists = doesAllVersionsMatch == false,
                Version = new ValueVersion
                {
                    InstanceId = instanceId,
                    Version = version.Value
                }
            };
        }

        private bool DoesAllVersionsMatch(string key, ValueVersion[] parentVersions)
        {
            var activeVersions = GatherActiveVersion(key);

            if (activeVersions.Length != parentVersions.Length)
                return false;

            activeVersions = activeVersions
                .OrderBy(x => x)
                .ToArray();

            parentVersions = parentVersions.OrderBy(x => x).ToArray();

            for (int i = 0; i < activeVersions.Length; i++)
            {
                if(activeVersions[i].Version != parentVersions[i].Version || 
                    activeVersions[i].InstanceId != parentVersions[i].InstanceId)
                    return false;
            }
            return true;
        }

        public Value[] Get(string key)
        {
            var values = new List<Value>();
            var activeVersions = GatherActiveVersion(key);

            bool foundAllInCache = true;
            foreach (var activeVersion in activeVersions)
            {
                var cachedValue = cache[GetKey(key, activeVersion)] as Value;
                if (cachedValue == null ||
                    (cachedValue.ExpiresAt.HasValue &&
                    DateTime.Now < cachedValue.ExpiresAt.Value))
                {
                    values.Clear();
                    foundAllInCache = false;
                    break;
                }
                values.Add(cachedValue);
            }
            if (foundAllInCache)
                return values.ToArray();

            ApplyToKeyAndActiveVersions(data, activeVersions, key, version =>
            {
                var value = ReadValueFromDataTable(version, key);

                if (value != null)
                    values.Add(value);
                else
                    commitSyncronization.Add(() => cache[GetKey(key, version)] = DBNull.Value);
            });

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

        private Value ReadValueFromDataTable(ValueVersion version, string key)
        {
            var expiresAtBinary = Api.RetrieveColumnAsDouble(session, data, dataColumns["expiresAt"]);
            DateTime? expiresAt = null;
            if (expiresAtBinary.HasValue)
            {
                expiresAt = DateTime.FromOADate(expiresAtBinary.Value);
                if (DateTime.Now > expiresAt)
                    return null;
            }
            var versions = new List<int>();
            using (var stream = new ColumnStream(session, data, dataColumns["parentVersions"]))
            {
                var parentVersion = ReadInt32(stream);
                while (parentVersion != null)
                {
                    versions.Add(parentVersion.Value);
                    stream.Itag += 1;
                    parentVersion = ReadInt32(stream);
                }
            }
            return new Value
            {
                Version = version,
                Key = key,
                ParentVersions = versions.ToArray(),
                Data = Api.RetrieveColumn(session, data, dataColumns["data"]),
                Sha256Hash = Api.RetrieveColumn(session, data, dataColumns["sha256_hash"]),
                ExpiresAt = expiresAt
            };
        }

        private static int? ReadInt32(Stream stream)
        {
            var buffer = new byte[sizeof(int)];
            var indexToStartReading = 0;
            do
            {
                var readBytes = stream.Read(buffer, indexToStartReading, buffer.Length);
                if (readBytes == 0)
                    return null;
                indexToStartReading += readBytes;
            } while (indexToStartReading < buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        public Value Get(string key, ValueVersion specifiedVersion)
        {
            var cachedValue = cache[GetKey(key, specifiedVersion)];
            if (cachedValue != null &&
                cachedValue != DBNull.Value)
                return (Value)cachedValue;

            Value val = null;
            ApplyToKeyAndActiveVersions(data, new[] { specifiedVersion }, key, version =>
            {
                val = ReadValueFromDataTable(specifiedVersion, key);
            });
            cache[GetKey(key, specifiedVersion)] = (object)val ?? DBNull.Value;
            return val;
        }

        private string GetKey(string key, ValueVersion version)
        {
            return GetKey(key) + "#" +
                version.InstanceId + "/" +
                version.Version;
        }

        private string GetKey(string key)
        {
            return "rhino.dht [" + database + "]: " + key;
        }

        public void Commit()
        {
            CleanExpiredValues();
            transaction.Commit(CommitTransactionGrbit.None);
            foreach (var action in commitSyncronization)
            {
                action();
            }
        }

        private void CleanExpiredValues()
        {
            Api.JetSetCurrentIndex(session, keys, "by_expiry");
            Api.MakeKey(session, keys, DateTime.Now.ToOADate(), MakeKeyGrbit.NewKey);

            if (Api.TrySeek(session, keys, SeekGrbit.SeekLT) == false)
                return;

            do
            {
                var key = Api.RetrieveColumnAsString(session, keys, keysColumns["key"], Encoding.Unicode);
                var version = ReadVersion();

                Api.JetDelete(session, keys);

                ApplyToKeyAndActiveVersions(data, new[] { version }, key, v => Api.JetDelete(session, data));

            } while (Api.TryMovePrevious(session, keys));
        }

        private ValueVersion ReadVersion()
        {
            var versionNumber = Api.RetrieveColumnAsInt32(session, keys, keysColumns["version_number"]).Value;
            var versionInstanceId = Api.RetrieveColumn(session, keys, keysColumns["version_instance_id"]);

            return new ValueVersion
            {
                InstanceId = new Guid(versionInstanceId),
                Version = versionNumber
            };
        }

        private void DeleteInactiveVersions(string key, IEnumerable<ValueVersion> versions)
        {
            ApplyToKeyAndActiveVersions(keys, versions, key,
                version => Api.JetDelete(session, keys));

            ApplyToKeyAndActiveVersions(data, versions, key, version =>
                Api.JetDelete(session, data));
        }

        private void ApplyToKeyAndActiveVersions(Table table, IEnumerable<ValueVersion> versions, string key, Action<ValueVersion> action)
        {
            Api.JetSetCurrentIndex(session, table, "pk");
            foreach (var version in versions)
            {
                Api.MakeKey(session, table, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
                Api.MakeKey(session, table, version.Version, MakeKeyGrbit.None);
                Api.MakeKey(session, table, version.InstanceId.ToByteArray(), MakeKeyGrbit.None);

                if (Api.TrySeek(session, table, SeekGrbit.SeekEQ) == false)
                    continue;

                action(version);
            }
        }

        private ValueVersion[] GatherActiveVersion(string key)
        {
            var cachedActiveVersions = cache[GetKey(key)];
            if (cachedActiveVersions != null)
                return (ValueVersion[])cachedActiveVersions;

            Api.JetSetCurrentIndex(session, keys, "by_key");
            Api.MakeKey(session, keys, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
            var exists = Api.TrySeek(session, keys, SeekGrbit.SeekEQ);
            if (exists == false)
                return new ValueVersion[0];

            Api.MakeKey(session, keys, key, Encoding.Unicode, MakeKeyGrbit.NewKey);
            Api.JetSetIndexRange(session, keys,
                SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive);

            var ids = new List<ValueVersion>();
            do
            {
                var version = ReadVersion();

                ids.Add(version);
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

            if (session != null)
                session.Dispose();
        }

        public bool Remove(string key, ValueVersion[] parentVersions)
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