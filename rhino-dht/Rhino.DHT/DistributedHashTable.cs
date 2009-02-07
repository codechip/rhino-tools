using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple
        )]
    public class DistributedHashTable : IDistributedHashTable
    {
        readonly PersistentHashTable hashTable;

        public DistributedHashTable()
            : this("cache.esent", null)
        {
        }

        public DistributedHashTable(string database)
            : this(database, null)
        {

        }

        public DistributedHashTable(string database, Action<InstanceParameters> configure)
        {
            try
            {
                hashTable = new PersistentHashTable(database)
                {
                    Configure = configure
                };
                hashTable.Initialize();
            }
            catch (Exception)
            {
                if (hashTable != null)
                    hashTable.Dispose();
                throw;
            }
        }

        public PutResult[] Put(AddValue[] valuesToAdd)
        {
            var versions = new List<PutResult>();
            hashTable.Batch(actions =>
            {
                foreach (var value in valuesToAdd)
                {
                	var version = actions.Put(
                		value.Key,
                        value.ParentVersions ?? new ValueVersion[0],
                		value.Bytes,
                		value.ExpiresAt,
                		value.OptimisticConcurrency == false);
                    versions.Add(version);
                }
                actions.Commit();
            });
            return versions.ToArray();
        }

        public Value[][] Get(GetValue[] valuesToGet)
        {
            var values = new List<Value[]>();
            hashTable.Batch(actions =>
            {
                foreach (var value in valuesToGet)
                {
                    if (value.SpecifiedVersion == null)
                    {
                        var version = actions.Get(value.Key);
                        values.Add(version);
                    }
                    else
                    {
                        var version = actions.Get(
                            value.Key,
                            value.SpecifiedVersion);
                        values.Add(new[] { version, });
                    }
                }
                actions.Commit();
            });
            return values.ToArray();
        }

        public bool[] Remove(RemoveValue[] valuesToRemove)
        {
            var values = new List<bool>();
            hashTable.Batch(actions =>
            {
                foreach (var value in valuesToRemove)
                {
                    var removed = actions.Remove(
                        value.Key,
                        value.ParentVersions ?? new ValueVersion[0]);
                    values.Add(removed);
                }
                actions.Commit();
            });
            return values.ToArray();
        }

        public void Dispose()
        {
            hashTable.Dispose();
        }
    }
}