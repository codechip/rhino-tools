using System;
using System.IO;
using System.ServiceModel;
using Microsoft.Isam.Esent.Interop;

namespace Rhino.DHT.Tests
{
    public class RemoteAppDomainRunner : MarshalByRefObject
    {
        private ServiceHost host;

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
            var uri = new Uri(Uri);

            host = new ServiceHost(new DistributedHashTable(File, Configure), uri);
            host.AddServiceEndpoint(typeof(IDistributedHashTable), new NetTcpBinding(), uri);
            host.Open();
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
        }
    }
}