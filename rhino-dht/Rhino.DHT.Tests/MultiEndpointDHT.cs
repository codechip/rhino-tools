using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class MultiEndpointDHT : IDisposable
    {
        private readonly MultiEndpointDistributedHashTable distributedHashTable;
        private readonly RemoteAppDomainRunner[] runners;

        public MultiEndpointDHT()
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

        private void Delete(string database)
        {
            if (Directory.Exists(database))
                Directory.Delete(database, true);
       
        }

        [Fact]
        public void Can_add_and_recieve_items_from_multiple_endpoints()
        {
            var versions = distributedHashTable.Put(new[]
            {
                new AddValue
                {
                    Key = "test74", 
                    Bytes = new byte[] {74}
                },
                new AddValue
                {
                    Key = "test75", 
                    Bytes = new byte[] {75}
                },
                new AddValue
                {
                    Key = "test77", 
                    Bytes = new byte[] {77}
                },
            });

            Assert.Equal(new[] {1, 1, 1}, versions.Select(x=>x.Version.Version).ToArray());

            var values = distributedHashTable.Get(new[]
            {
                new GetValue
                {
                    Key = "test74",
                },
                new GetValue
                {
                    Key = "test75",
                },
                new GetValue
                {
                    Key = "test77",
                },
            });
            Assert.Equal(3, values.Length);
            Assert.Equal(new byte[] { 74}, values[0][0].Data);
            Assert.Equal(new byte[] { 75 }, values[1][0].Data);
            Assert.Equal(new byte[] { 77 }, values[2][0].Data);
        }

        [Fact]
        public void Can_add_and_remove_items_from_multiple_endpoints()
        {
            var versions = distributedHashTable.Put(new[]
            {
                new AddValue
                {
                    Key = "test74", 
                    Bytes = new byte[] {74}
                },
                new AddValue
                {
                    Key = "test75", 
                    Bytes = new byte[] {75}
                },
                new AddValue
                {
                    Key = "test77", 
                    Bytes = new byte[] {77}
                },
            });

            Assert.Equal(new[] { 1, 1, 1 }, versions.Select(e => e.Version.Version).ToArray());

            var removed = distributedHashTable.Remove(new[]
            {
                new RemoveValue()
                {
                    Key = "test74",
                    ParentVersions = new []{versions[0].Version}
                },
                new RemoveValue
                {
                    Key = "test75",
                    ParentVersions = new []{versions[1].Version}
                },
                new RemoveValue
                {
                    Key = "test77",
                    ParentVersions = new []{versions[2].Version}
                },
            });

            Assert.Equal(new[] { true, true, true }, removed);

            var values = distributedHashTable.Get(new[]
            {
                new GetValue
                {
                    Key = "test74",
                },
                new GetValue
                {
                    Key = "test75",
                },
                new GetValue
                {
                    Key = "test77",
                },
            });
            Assert.Equal(3, values.Length);
            Assert.Equal(0, values[0].Length);
            Assert.Equal(0, values[1].Length);
            Assert.Equal(0, values[2].Length);
        }

        public void Dispose()
        {
            distributedHashTable.Dispose();
            foreach (var host in runners)
            {
                host.Close();
                AppDomain.Unload(host.AppDomain);
            }
        }
    }
}