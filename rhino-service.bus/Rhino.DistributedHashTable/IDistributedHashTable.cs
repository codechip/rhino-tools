using System;
using System.ServiceModel;

namespace Rhino.DistributedHashTable
{
	using PersistentHashTable;

	[ServiceContract]
	public interface IDistributedHashTable : IDisposable
	{
		Uri Url { get; }

		[OperationContract]
		PutResult[] Put(Uri originalDestination, params PutRequest[] valuesToAdd);

		[OperationContract]
		Value[][] Get(params GetRequest[] valuesToGet);

		[OperationContract]
		bool[] Remove(Uri originalDestination, params RemoveRequest[] valuesToRemove);
	}
}