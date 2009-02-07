using System;
using System.Collections.Generic;
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
                var versionForCurrentBatch = WriteValuesToNode(values, array);
                for (int i = 0; i < array.Length; i++)
                {
                    versions[Array.IndexOf(valuesToAdd, array[i])] = versionForCurrentBatch[i];
                }
            }
            return versions;
        }

        private PutResult[] WriteValuesToNode(IGrouping<IDistributedHashTable, AddValue> values, AddValue[] array)
        {
            try
            {
                return values.Key.Put(array);
            }
            catch (CommunicationException e)
            {
                 // endpoint is down, so we want to try on the other endpoints
                foreach (var otherEndpoint in endpoints.GetOtherElementsFromElement(values.Key))
                {
                    try
                    {
                        return otherEndpoint.Put(array);
                    }
                    catch (CommunicationException)
                    {
                        // still an error, let us try another node
                    }
                }

                throw new InvalidOperationException("could not write to any of the nodes in the DHT, tried ["+endpoints.Length + "] nodes", e);
            }
        }

        public Value[][] Get(params GetValue[] valuesToGet)
        {
            var groupedByUri = from x in valuesToGet
                               group x by GetUrl(x.Key);
            var valuesFromEndpoints = new Value[valuesToGet.Length][];
            foreach (var values in groupedByUri)
            {
                var array = values.ToArray();
                var valuesFromCurrentBatch = GetValuesFromEndpoint(values, array);
                for (var i = 0; i < array.Length; i++)
                {
                    valuesFromEndpoints[Array.IndexOf(valuesToGet, array[i])] = valuesFromCurrentBatch[i];
                }
            }
            return valuesFromEndpoints;
        }

        private Value[][] GetValuesFromEndpoint(
            IGrouping<IDistributedHashTable, GetValue> values,
            GetValue[] valuesToGet)
        {
            try
            {
                return values.Key.Get(valuesToGet);
            }
            catch (CommunicationException)
            {
                // endpoint is down, so we want to try on the other endpoints
                // for the replicated value
                foreach (var otherEndpoint in endpoints.GetOtherElementsFromElement(values.Key))
                {
                    try
                    {
                        var results = otherEndpoint.Get(valuesToGet);
                        // we got _some_ value here
                        // trying to merge all the results from all the endpoints would be 
                        // too hard, so we just check if we got any values.
                        // if we do, we consider this okay. 
                        // considering that this will only happen in failure mode, and that the 
                        // scenario will only cause problems when we have multi keys request, I 
                        // think this is a valid approach.
                        // it is not likely that the other endpoints in the DHT will know about the
                        // other values either, so this is a good way of breaking it down
                        if (results.Any(x => x.Length > 0))
                        {
                            return results;
                        }
                    }
                    catch (CommunicationException)
                    {
                        // another endpoint down, let us try with another one
                    }
                }
                var emptyResults = new Value[valuesToGet.Length][];
                for (int i = 0; i < emptyResults.Length; i++)
                {
                    emptyResults[i] = new Value[0];
                }
                return emptyResults;
            }
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