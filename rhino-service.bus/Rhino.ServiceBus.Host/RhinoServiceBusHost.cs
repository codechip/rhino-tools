using System.ServiceProcess;

namespace Rhino.ServiceBus.Host
{
	using Hosting;

	partial class RhinoServiceBusHost : ServiceBase
	{
		private RemoteAppDomainHost host;

		public RhinoServiceBusHost()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			host = new RemoteAppDomainHost(args[0]);
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
