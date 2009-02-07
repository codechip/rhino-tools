using System;
using System.ServiceModel;

namespace Rhino.DHT
{
    [ServiceContract]
    public interface IDistributedHashTable : IDisposable
    {
        [OperationContract]
        PutResult[] Put(params AddValue[] valuesToAdd);

        [OperationContract]
        Value[][] Get(params GetValue[] valuesToGet);

        [OperationContract]
        bool[] Remove(params RemoveValue[] valuesToRemove);

        [OperationContract]
        void Replicate(ReplicationValue[] valuesToReplicate);

        [OperationContract]
        void RegisterNodes(Uri[] uris);
    }
}