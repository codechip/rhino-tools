using System;
using Rhino.ServiceBus.LoadBalancer;

namespace Rhino.ServiceBus.Hosting
{
    public class RemoteAppDomainLoadBalancerHost : RemoteAppDomainHost
    {
        public RemoteAppDomainLoadBalancerHost(Type boosterType) : base(boosterType)
        {
        }

        public RemoteAppDomainLoadBalancerHost(string assemblyPath) : base(assemblyPath)
        {
        }

        protected override HostedService CreateRemoteHost(AppDomain appDomain)
        {
            object instance = appDomain.CreateInstanceAndUnwrap("Rhino.ServiceBus",
                                                                "Rhino.ServiceBus.LoadBalancer.LoadBalancerHost");
            var hoster = (LoadBalancerHost)instance;

            return new HostedService
            {
                Stop = () =>
                {
                    hoster.Dispose();
                    AppDomain.Unload(appDomain);
                },
                Start = hoster.Start
            };
        }
    }
}