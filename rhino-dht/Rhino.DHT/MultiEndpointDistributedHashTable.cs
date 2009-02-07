using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Linq;

namespace Rhino.DHT
{
    public class MultiEndpointDistributedHashTable
    {
        private readonly IDistributedHashTable[] endpoints;

        public IDistributedHashTable[] Endpoints
        {
            get { return endpoints; }
        }

        public MultiEndpointDistributedHashTable(Uri[] uris, Binding binding)
        {
            endpoints = new IDistributedHashTable[uris.Length];
            try
            {
                for (var i = 0; i < uris.Length; i++)
                {
                    endpoints[i] = ChannelFactory<IDistributedHashTable>
                        .CreateChannel(binding, new EndpointAddress(uris[i]));

                    var copy = i;
                    endpoints[i].RegisterNodes(
                        uris
                        .Where(x => x != uris[copy])
                        .ToArray()
                        );
                }
            }
            catch (Exception)
            {
                foreach (ICommunicationObject endpoint in endpoints)
                {
                    if (endpoint == null)
                        continue;
                    endpoint.Close();
                }
            }
        }

        public PutResult[] Put(params AddValue[] valuesToAdd)
        {
            var groupedByUri = from x in valuesToAdd
                               group x by GetUrl(x.Key);
            var versions = new PutResult[valuesToAdd.Length];
            foreach (var values in groupedByUri)
            {
                var array = values.ToArray();
                var versionForCurrentBatch = values.Key.Put(array);
                for (int i = 0; i < array.Length; i++)
                {
                    versions[Array.IndexOf(valuesToAdd, array[i])] = versionForCurrentBatch[i];
                }
            }
            return versions;
        }

        public Value[][] Get(params GetValue[] valuesToGet)
        {
            var groupedByUri = from x in valuesToGet
                               group x by GetUrl(x.Key);
            var valuesFromEndpoints = new Value[valuesToGet.Length][];
            foreach (var values in groupedByUri)
            {
                var array = values.ToArray();
                var valuesFromCurrentBatch = values.Key.Get(array);
                for (int i = 0; i < array.Length; i++)
                {
                    valuesFromEndpoints[Array.IndexOf(valuesToGet, array[i])] = valuesFromCurrentBatch[i];
                }
            }
            return valuesFromEndpoints;
        }

        public bool[] Remove(params RemoveValue[] valuesToRemove)
        {
            var groupedByUri = from x in valuesToRemove
                               group x by GetUrl(x.Key);
            var valuesFromEndpoints = new bool[valuesToRemove.Length];
            foreach (var values in groupedByUri)
            {
                var array = values.ToArray();
                var valuesFromCurrentBatch = values.Key.Remove(array);
                for (int i = 0; i < array.Length; i++)
                {
                    valuesFromEndpoints[Array.IndexOf(valuesToRemove, array[i])] = valuesFromCurrentBatch[i];
                }
            }
            return valuesFromEndpoints;
        }

        private IDistributedHashTable GetUrl(string key)
        {
            var index = Math.Abs(key.GetHashCode()) % endpoints.Length;
            return endpoints[index];
        }

        public void Dispose()
        {
            foreach (ICommunicationObject endpoint in endpoints)
            {
                try
                {
                    endpoint.Close();
                }
                catch
                {
                }
            }
        }
    }
}