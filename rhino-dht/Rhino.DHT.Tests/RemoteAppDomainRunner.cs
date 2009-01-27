using System;
using System.IO;
using System.ServiceModel;
using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT.Tests
{
    public class RemoteAppDomainRunner : MarshalByRefObject
    {
        private ServiceHost host;
        private DistributedHashTable instance;

        public static RemoteAppDomainRunner Start(string file, string uri)
        {
            var domain = AppDomain.CreateDomain(file,null, new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            });
            var unwrap = (RemoteAppDomainRunner)domain.CreateInstanceAndUnwrap(
                typeof(RemoteAppDomainRunner).Assembly.FullName, 
                typeof(RemoteAppDomainRunner).FullName);

            unwrap.File = file;
            unwrap.Uri = uri;
            unwrap.Start();

            return unwrap;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string File { get; set; }
        public string Uri { get; set; }

        public void Start()
        {
            try
            {
                var uri = new Uri(Uri);

                instance = new DistributedHashTable(File, Configure);
                host = new ServiceHost(instance, uri);
                host.AddServiceEndpoint(typeof(IDistributedHashTable), new NetTcpBinding(), uri);
                host.Open();
            }
            catch (EsentException e)
            {
                throw new Exception(e.ToString());
            }
        }

        public void Configure(InstanceParameters ip)
        {
            ip.SystemDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            ip.TempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            ip.LogFileDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(ip.SystemDirectory);
            Directory.CreateDirectory(ip.TempDirectory);
            Directory.CreateDirectory(ip.LogFileDirectory);
        }

        public AppDomain AppDomain
        {
            get { return System.AppDomain.CurrentDomain; }
        }

        public void Close()
        {
            host.Close();
            instance.Dispose();
        }
    }
}