using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class ReplicationFixture : IDisposable
    {
        private readonly MultiEndpointDistributedHashTable distributedHashTable;
        private readonly RemoteAppDomainRunner[] runners;

        public ReplicationFixture()
        {
            Delete("cache1.esent");
            Delete("cache2.esent");
            Delete("cache3.esent");

            runners = new[]
            {
                RemoteAppDomainRunner.Start("cache1.esent","net.tcp://localhost:6212"),
                RemoteAppDomainRunner.Start("cache2.esent","net.tcp://localhost:6213"),
                RemoteAppDomainRunner.Start("cache3.esent","net.tcp://localhost:6214")
            };

            distributedHashTable = new MultiEndpointDistributedHashTable(
                runners.Select(x => new Uri(x.Uri)).ToArray(),
                new NetTcpBinding());
        }

        private static void Delete(string database)
        {
            if (Directory.Exists(database))
                Directory.Delete(database, true);
        }

        [Fact]
        public void Will_register_all_other_nodes()
        {
            foreach (var runner in runners)
            {
                var destinations = runner.GetReplicationDestinations();
                var copy = runner;
                var actual = destinations.OrderBy(x => x).Select(x => new Uri(x)).ToArray();
                var expected = runners.Where(x => x != copy).Select(x => new Uri(x.Uri)).ToArray();
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void When_saving_item_in_one_node_will_be_replicated_to_all_others()
        {
            distributedHashTable.Put(new AddValue
            {
                Bytes = new byte[] { 1, 2, 3, 4, 5 },
                ParentVersions = new ValueVersion[0],
                Key = "test"
            });


            WaitForValueInEndpoint(distributedHashTable.Endpoints[0]);
            WaitForValueInEndpoint(distributedHashTable.Endpoints[1]);
            WaitForValueInEndpoint(distributedHashTable.Endpoints[2]);
        }

        [Fact]
        public void When_removing_item_in_one_node_will_be_replicated_to_all_others()
        {
            var results = distributedHashTable.Put(new AddValue
            {
                Bytes = new byte[] { 1, 2, 3, 4, 5 },
                ParentVersions = new ValueVersion[0],
                Key = "test"
            });


            WaitForValueInEndpoint(distributedHashTable.Endpoints[0]);
            WaitForValueInEndpoint(distributedHashTable.Endpoints[1]);
            WaitForValueInEndpoint(distributedHashTable.Endpoints[2]);

            distributedHashTable.Remove(new RemoveValue
            {
                Key = "test",
                ParentVersions = new[] { results[0].Version }
            });

            WaitForValueRemoval(distributedHashTable.Endpoints[0]);
            WaitForValueRemoval(distributedHashTable.Endpoints[1]);
            WaitForValueRemoval(distributedHashTable.Endpoints[2]);
        }

        private static void WaitForValueRemoval(IDistributedHashTable endpoint)
        {
            while (true)
            {
                var values = endpoint.Get(new GetValue
                {
                    Key = "test"
                });
                if (values[0].Length == 0)
                    return;
                Thread.Sleep(500);
            }
        }


        private static void WaitForValueInEndpoint(IDistributedHashTable endpoint)
        {
            while (true)
            {
                var values = endpoint.Get(new GetValue
                {
                    Key = "test"
                });
                if (values[0].Length == 0)
                    continue;
                Assert.Equal(values[0][0].Data, new byte[] { 1, 2, 3, 4, 5 });
                Thread.Sleep(500);
                return;
            }
        }

        public void Dispose()
        {
            foreach (var runner in runners)
            {
                runner.Close();
            }
            distributedHashTable.Dispose();
        }
    }
}