namespace Rhino.DistributedHashTable
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Transactions;
	using PersistentHashTable;
	using ServiceBus.Internal;
	using ServiceBus;
	using Util;

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Multiple,
		InstanceContextMode = InstanceContextMode.Single,
		IncludeExceptionDetailInFaults = true
		)]
	public class DistributedHashTable : IDistributedHashTable
	{
		private readonly Uri url;

		private readonly IEndpointRouter endpointRouter;

		private readonly IServiceBus bus;

		const string rhinoDhtStartToken = "@rdht://";

		public Node Metadata { get; set; }

		private readonly PersistentHashTable hashTable;

		private Node failOver;

		public DistributedHashTable(
			string database,
			Uri url,
			IEndpointRouter endpointRouter,
			IServiceBus bus)
			: this(database, url, endpointRouter, bus, null)
		{

		}

		public DistributedHashTable(
			string database,
			Uri url,
			IEndpointRouter endpointRouter,
			IServiceBus bus,
			Node metadata)
		{
			this.url = url;
			this.endpointRouter = endpointRouter;
			this.bus = bus;
			Metadata = metadata;

			if (Metadata != null) // sole node in the network, probably
			{
				Metadata.ExecuteSync((uri,original) =>
				{
					ServiceUtil.Execute<IDistributedHashTableMetaDataProvider>(uri, srv =>
					{
						failOver = srv.GetNodeByUri(url);
					});
				});
			}
			try
			{
				hashTable = new PersistentHashTable(database);
				hashTable.Initialize();
			}
			catch (Exception)
			{
				hashTable.Dispose();
				throw;
			}
		}

		public Uri Url
		{
			get { return url; }
		}

		public void Dispose()
		{
			hashTable.Dispose();
		}

		public PutResult[] Put(Uri originalDestination, params PutRequest[] valuesToAdd)
		{
			var results = new List<PutResult>();
			using (var tx = new TransactionScope())
			{
				hashTable.Batch(actions =>
				{
					foreach (var request in valuesToAdd)
					{
						if(request.ParentVersions==null)
							throw new ArgumentException("Could not accept request with no ParentVersions");
						if (request.Key.StartsWith(rhinoDhtStartToken))
							throw new ArgumentException(rhinoDhtStartToken + " is a reserved key prefix");
						var put = actions.Put(request);
						results.Add(put);

						SendToFailoverNodes(request);
					}
					if (originalDestination != url)
						SendTo(originalDestination, valuesToAdd);
					actions.Commit();
				});

				tx.Complete();
			}
			return results.ToArray();
		}

		private void SendToFailoverNodes(object msg)
		{
			if (failOver == null)
				return;
			if (failOver.Secondary != null)
				bus.Send(endpointRouter.GetRoutedEndpoint(failOver.Secondary.Async), msg);
			if (failOver.Tertiary != null)
				bus.Send(endpointRouter.GetRoutedEndpoint(failOver.Tertiary.Async), msg);
		}

		private void SendTo(Uri originalDestination, object[] msgs)
		{
			bus.Send(endpointRouter.GetRoutedEndpoint(originalDestination), msgs);
		}

		public bool[] Remove(Uri originalDestination, params RemoveRequest[] valuesToRemove)
		{

			var results = new List<bool>();
			using (var tx = new TransactionScope())
			{
				hashTable.Batch(actions =>
				{
					foreach (var request in valuesToRemove)
					{
						if (request.ParentVersions == null)
							throw new ArgumentException("Could not accept request with no ParentVersions");
						
						var remove = actions.Remove(request);
						results.Add(remove);

						SendToFailoverNodes(request);
					}
					if (originalDestination != url)
						SendTo(originalDestination, valuesToRemove);
					actions.Commit();
				});

				tx.Complete();
			}
			return results.ToArray();
		}

		public Value[][] Get(params GetRequest[] valuesToGet)
		{
			var results = new List<Value[]>();
			hashTable.Batch(actions =>
			{
				foreach (var request in valuesToGet)
				{
					var values = actions.Get(request);
					results.Add(values);
				}

				actions.Commit();
			});
			return results.ToArray();
		}
	}
}