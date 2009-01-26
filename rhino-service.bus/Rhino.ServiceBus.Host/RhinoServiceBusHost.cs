using Rhino.ServiceBus.LoadBalancer;

namespace Rhino.ServiceBus.Host
{
	using System.ServiceProcess;
	using System.Threading;
	using Hosting;

	internal partial class RhinoServiceBusHost : ServiceBase
	{
		private RemoteAppDomainHost host;
		private string asm;
	    private string cfg;
	    private bool loadBalancer;

	    public RhinoServiceBusHost()
		{
			InitializeComponent();
		}

		public void SetArguments(string assembly, string config, bool useLoadBalancer)
		{
		    asm = assembly;
		    cfg = config;
		    loadBalancer = useLoadBalancer;
		}

		protected override void OnStart(string[] ignored)
		{
            host = loadBalancer ?
                new RemoteAppDomainLoadBalancerHost(asm, cfg) :
                new RemoteAppDomainHost(asm, cfg);
			host.Start();
		}

		protected override void OnStop()
		{
			if (host != null)
				host.Close();
		}

		public void DebugStart(string[] arguments)
		{
			OnStart(arguments);
		}
	}
}