using System.ServiceModel;

namespace Rhino.DHT
{
    [ServiceContract]
    public interface IDistributedHashTable
    {
        [OperationContract]
        int[] Put(AddValue[] valuesToAdd);

        [OperationContract]
        Value[][] Get(GetValue[] valuesToGet);

        [OperationContract]
        bool[] Remove(RemoveValue[] valuesToRemove);
    }
}