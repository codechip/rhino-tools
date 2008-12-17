using System;
using System.IO;
using System.Threading;
using log4net;

namespace Rhino.ServiceBus.Hosting
{
    public class RemoteAppDomainHost
    {
        private readonly string assembly;
        private readonly ILog logger = LogManager.GetLogger(typeof (RemoteAppDomainHost));
        private readonly string path;
        private HostedService current;

        public RemoteAppDomainHost(string assemblyPath)
        {
            assembly = Path.GetFileNameWithoutExtension(assemblyPath);
            path = Path.GetDirectoryName(assemblyPath);
        }

        public void Start()
        {
            HostedService service = CreateNewAppDomain(path, assembly);
            var watcher = new FileSystemWatcher(path);
            bool wasCalled = false;
            var @lock = new object();
            FileSystemEventHandler handler = (sender, e) =>
            {
                string extension = Path.GetExtension(e.FullPath);
                if (extension != ".dll" && extension != ".config")
                    return;
                watcher.Dispose();
                lock (@lock)
                {
                    if (wasCalled)
                        return;
                    wasCalled = true;

                    logger.WarnFormat("Got change request for {0}, disposing current AppDomain",
                                      e.Name);
                    service.Stop();

                    Thread.Sleep(500); //allow for other events to happen
                    logger.Warn("Restarting...");
                    Start();
                }
            };
            watcher.Deleted += handler;
            watcher.Changed += handler;
            watcher.Created += handler;

            watcher.EnableRaisingEvents = true;

            current = service;

            service.Start();
        }

        private static HostedService CreateNewAppDomain(string path, string assembly)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = path,
                ApplicationName = assembly,
                ConfigurationFile = Path.Combine(path, assembly + ".dll.config"),
                ShadowCopyFiles = "true" //yuck
            };
            AppDomain appDomain = AppDomain.CreateDomain(assembly, null, appDomainSetup);
            object instance = appDomain.CreateInstanceAndUnwrap("Rhino.ServiceBus",
                                                                "Rhino.ServiceBus.Hosting.DefaultHost");
            var hoster = (DefaultHost) instance;

            return new HostedService
            {
                Stop = hoster.Close,
                Start = () => hoster.Start(assembly)
            };
        }

        public void Close()
        {
            if (current != null)
                current.Stop();
        }

        #region Nested type: HostedService

        private class HostedService
        {
            public Action Start;
            public Action Stop;
        }

        #endregion
    }
}