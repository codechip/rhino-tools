using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoTorrent.Client;
using MonoTorrent.Common;

namespace HibernatingTorrent
{
	public class TorrentServer : IDisposable
	{
		private readonly ClientEngine engine;
		private readonly string downloadsPath;

		readonly ManualResetEvent WaitForDisposable = new ManualResetEvent(false);

		public TorrentServer(string downloadsPath, int listenPort)
		{
			this.downloadsPath = downloadsPath;
			var engineSettings = new EngineSettings(downloadsPath, listenPort);
			engine = new ClientEngine(engineSettings);

		}

		public void Start()
		{
			foreach (string file in Directory.GetFiles(downloadsPath, "*.torrent"))
			{
				Torrent torrent = Torrent.Load(file);
				string savePath = Path.Combine(downloadsPath, torrent.Name);
				var manager = new TorrentManager(torrent, savePath, new TorrentSettings());
				manager.TorrentStateChanged +=
					delegate(object sender, TorrentStateChangedEventArgs e)
					{
							if(e.NewState==TorrentState.Seeding)
							{
								ReportSeeding(e.TorrentManager.Torrent.Name);
							}
						 Console.WriteLine("Torrent {0} changed: {1} -> {2}", e.TorrentManager.Torrent.Name, e.OldState, e.NewState);
					};
				manager.PeerConnected +=
					delegate(object sender, PeerConnectionEventArgs e)
					{
						Console.WriteLine("Peer {2} connect to: {0} as {1}", e.TorrentManager.Torrent.Name, e.ConnectionDirection,
						                  e.PeerID.Location);
					};
				engine.Register(manager);
			}
			engine.StartAll();
			WaitForDisposable.WaitOne();
		}

		private void ReportSeeding(string name)
		{
			EventLog.WriteEntry("HibernatingTorrent", "Starting to seed: " + name, EventLogEntryType.Information);
		}

		#region IDisposable Members

		public void Dispose()
		{
			engine.Dispose();
			WaitForDisposable.Reset();
		}

		#endregion
	}
}