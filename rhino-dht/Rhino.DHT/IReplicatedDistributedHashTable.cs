using System.ServiceModel;

namespace Rhino.DHT
{
    [ServiceContract]
    public interface IReplicatedDistributedHashTable
    {
        [OperationContract]
        void Replicate(ReplicationValue[] valuesToReplicate);
    }
}