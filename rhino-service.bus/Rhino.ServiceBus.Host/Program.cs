namespace Rhino.ServiceBus.Host
{
	using System;
	using System.Collections;
	using System.Configuration.Install;
	using System.Linq;
	using System.ServiceProcess;
	using Microsoft.Win32;

	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe debug [assembly]");
				Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe service [assembly]");
				Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe install [name] [assembly]");
				Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe uninstall [name] [assembly]");
				return;
			}

			try
			{
				var action = args[0];

				if (action == "install" || args[0] == "uninstall")
				{
					if (args.Length != 3)
					{
						Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe install [name] [assembly]");
						Console.WriteLine("Usage: Rhino.ServiceBus.Host.exe uninstall [name] [assembly]");
						return;
					}

					var installer = new ProjectInstaller
					{
						DisplayName = args[1],
						Description = args[1],
						Context = new InstallContext()
					};
					installer.Context.Parameters.Add("assemblypath", typeof(Program).Assembly.Location);
					if (action == "install")
					{
						installer.Install(new Hashtable());
						using (var system = Registry.LocalMachine.OpenSubKey("System"))
						using (var currentControlSet = system.OpenSubKey("CurrentControlSet"))
						using (var services = currentControlSet.OpenSubKey("Services"))
						using (var service = services.OpenSubKey(installer.ServiceName, true))
						{
							var path = (string)service.GetValue("ImagePath");
							path += " service " + string.Join(" ", args.Skip(2).ToArray());
							service.SetValue("ImagePath", path);
						}
					}
					else
					{
						installer.Uninstall(null);
					}

					return;
				}

				if (action == "debug")
				{
					var host = new RhinoServiceBusHost();
					host.SetArguments(args.Skip(1).ToArray());
					host.DebugStart(new string[0]);
					Console.WriteLine("Press any key to continue...");
					Console.ReadKey();
					host.Stop();
				}
				else
				{
					var host = new RhinoServiceBusHost();
					host.SetArguments(args.Skip(1).ToArray());
					ServiceBase.Run(host);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}