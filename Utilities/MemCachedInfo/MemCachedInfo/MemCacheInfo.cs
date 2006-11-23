using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MemCachedInfo.Properties;

namespace MemCachedInfo
{
	public partial class MemCacheInfo : Form
	{
		private Socket _socket;
		private DataTable stats;

		public MemCacheInfo()
		{
			InitializeComponent();
		}

		private void MemCacheInfo_Load(object sender, EventArgs e)
		{
			bgWorker.RunWorkerAsync();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (bgWorker.IsBusy == false)
				bgWorker.RunWorkerAsync();
		}

		private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				if (_socket == null)
				{
					_socket = CreateSocket();
				}
				stats = GetData();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private Socket CreateSocket()
		{
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			s.Connect(new IPEndPoint(IPAddress.Parse(Settings.Default.Address), Settings.Default.Port));
			return s;
		}

		private DataTable GetData()
		{
			DataTable dt = new DataTable("STATS");
			dt.Columns.Add("STAT", typeof (string));
			dt.Columns.Add("VAL", typeof (string));
			using (NetworkStream ns = new NetworkStream(_socket))
			{
				StreamWriter sw = new StreamWriter(ns);
				sw.WriteLine("stats");
				sw.Flush();

				StreamReader sr = new StreamReader(ns);
				string line;
				while ("END" != (line = sr.ReadLine()))
				{
					string[] items = line.Split(' ');
					DataRow row = dt.NewRow();
					row[0] = items[1];
					row[1] = items[2];
					if (items[1].Contains("bytes"))
					{
						decimal number = decimal.Parse(items[2]);
						if (number < 1024)
							row[1] = number + " bytes";
						else if (number < 1024*1024)
							row[1] = number/1024 + " KB";
						else
							row[1] = number/1024/1024 + " MB";
					}
					dt.Rows.Add(row);
				}
			}
			return dt;
		}

		private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			grid.DataSource = stats;
		}

		private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (Socket socket = CreateSocket())
			{
				using (NetworkStream ns = new NetworkStream(socket))
				{
					StreamWriter sw = new StreamWriter(ns);
					sw.WriteLine("flush_all");
					sw.Flush();
					ns.Flush();
					Thread.Sleep(750);
				}
			}
		}

		private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timer.Enabled = !timer.Enabled;
			if (timer.Enabled)
			{
				pauseToolStripMenuItem.Text = "&Pause";
			}
			else
			{
				pauseToolStripMenuItem.Text = "&Start";
			}
		}
	}
}