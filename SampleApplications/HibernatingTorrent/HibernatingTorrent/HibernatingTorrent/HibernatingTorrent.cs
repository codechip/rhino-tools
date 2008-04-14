using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using HibernatingTorrent.Properties;

namespace HibernatingTorrent
{
	public partial class HibernatingTorrent : ServiceBase
	{
		private TorrentServer torrentServer;

		public HibernatingTorrent()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			EventLog.WriteEntry("Starting torrent server", EventLogEntryType.Information);
			torrentServer = new TorrentServer(Settings.Default.DownloadDir, Settings.Default.Port);
			new Thread(torrentServer.Start).Start();
			EventLog.WriteEntry("Started torrent server", EventLogEntryType.Information);
		}

		protected override void OnStop()
		{
			torrentServer.Dispose();
		}
	}
}