using System;
using System.ServiceProcess;
using System.Threading;
using HibernatingTorrent.Properties;

namespace HibernatingTorrent
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		private static void Main(string[] args)
		{
			if (args.Length == 1 && args[0].Equals("debug", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("Serving: {0} on {1}",
					Settings.Default.DownloadDir,
					Settings.Default.Port);
				var torrentServer = new TorrentServer(Settings.Default.DownloadDir, Settings.Default.Port);
				new Thread(torrentServer.Start).Start();
				Console.WriteLine("Torrent Server started...");
				Console.ReadLine();
				torrentServer.Dispose();
				return;
			}

			ServiceBase.Run(new[] { new HibernatingTorrent() });
		}
	}
}