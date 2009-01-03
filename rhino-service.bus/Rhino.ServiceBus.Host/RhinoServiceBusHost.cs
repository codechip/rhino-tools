namespace Rhino.ServiceBus.Host
{
	using System.ServiceProcess;
	using System.Threading;
	using Hosting;

	internal partial class RhinoServiceBusHost : ServiceBase
	{
		private RemoteAppDomainHost host;
		private string[] args;

		public RhinoServiceBusHost()
		{
			InitializeComponent();
		}

		public void SetArguments(string[] arguments)
		{
			args = arguments;
		}

		protected override void OnStart(string[] ignored)
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