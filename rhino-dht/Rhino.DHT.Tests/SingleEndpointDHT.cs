using System;
using System.IO;
using System.ServiceModel;
using Xunit;

namespace Rhino.DHT.Tests
{
    public class SingleEndpointDHT : IDisposable
    {
        private readonly ServiceHost host;
        private readonly Uri address;

        public SingleEndpointDHT()
        {
            Delete("cache.esent");

            address = new Uri("net.tcp://localhost:6212/cache");
            host = new ServiceHost(typeof(DistributedHashTable), address);
            host.AddServiceEndpoint(typeof(IDistributedHashTable), new NetTcpBinding(), this.address);
            host.Open();
        }

        private void Delete(string database)
        {
            if (Directory.Exists(database))
                Directory.Delete(database, true);
       
        }

        [Fact]
        public void Can_add_and_recieve_item()
        {
            var distributedHashTable = ChannelFactory<IDistributedHashTable>
                .CreateChannel(
                    new NetTcpBinding(),
                    new EndpointAddress(address)
                );

            distributedHashTable.Put(new[]
            {
                new AddValue
                {
                    Key = "abc", 
                    Bytes = new byte[] {123}
                },
            });

            var values = distributedHashTable.Get(new[]
            {
                new GetValue
                {
                    Key = "abc",
                },
            });
            Assert.Equal(1, values[0].Length);
            Assert.Equal(new byte[] { 123 }, values[0][0].Data);
        }

        [Fact]
        public void Can_add_and_recieve_lots_of_items()
        {
            var distributedHashTable = ChannelFactory<IDistributedHashTable>
                .CreateChannel(
                    new NetTcpBinding(),
                    new EndpointAddress(address)
                );

            for (int i = 0; i < 500; i++)
            {
                distributedHashTable.Put(new[]
                {
                    new AddValue
                    {
                        Key = "abc" + i,
                        Bytes = new byte[] {123}
                    },
                });

                var values = distributedHashTable.Get(new[]
                {
                    new GetValue
                    {
                        Key = "abc" + i,
                    },
                });
                Assert.Equal(1, values[0].Length);
                Assert.Equal(new byte[] {123}, values[0][0].Data);
            }
        }


        [Fact]
        public void Can_remove_value()
        {
            var distributedHashTable = ChannelFactory<IDistributedHashTable>
                .CreateChannel(
                    new NetTcpBinding(),
                    new EndpointAddress(address)
                );

            var versions = distributedHashTable.Put(new[]
            {
                new AddValue
                {
                    Key = "abc", 
                    Bytes = new byte[] {123}
                },
            });

            var removed = distributedHashTable.Remove(new[]
            {
                new RemoveValue
                {
                    Key = "abc",
                    ParentVersions = versions
                },
            });
            Assert.True(removed[0]);

            var values = distributedHashTable.Get(new[]
            {
                new GetValue
                {
                    Key = "abc",
                },
            });
            Assert.Equal(0, values[0].Length);
        }

        public void Dispose()
        {
            host.Close();
        }
    }
}