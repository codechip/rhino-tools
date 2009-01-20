using System;
using System.ServiceModel;

namespace Rhino.DHT
{
    [ServiceContract]
    public interface IDistributedHashTable : IDisposable
    {
        [OperationContract]
        int[] Put(AddValue[] valuesToAdd);

        [OperationContract]
        Value[][] Get(GetValue[] valuesToGet);

        [OperationContract]
        bool[] Remove(RemoveValue[] valuesToRemove);
    }
}