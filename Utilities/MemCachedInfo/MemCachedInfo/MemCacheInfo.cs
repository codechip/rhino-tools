#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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