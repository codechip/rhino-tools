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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using CookComputing.XmlRpc;
using TracUI.Properties;

namespace TracUI
{
	public partial class MainView : Form
	{
		private int counter = 0;
		private bool initSuccessful = false;
		private string[] milestones;
		private string[] components_list;
		private string[] priorities;
		private string[] versions;
		private string[] severities;
		private bool shouldDraw;
		private Point temporaryOutlineStart;
		private bool currentlyDrawing;
		private Rectangle? temporaryOutline;

		public MainView()
		{
			InitializeComponent();
		}

		private void initMenues_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				ITracXmlRpc trac = BuildTracService();
				milestones = trac.GetMilestones();
				components_list = trac.GetComponents();
				priorities = trac.GetPriorities();
				severities = trac.GetSeverities();
				versions = trac.GetVersions();
				initSuccessful = true;
			}
			catch (Exception e1)
			{
				counter++;
				if (counter < 3)
				{
					initMenues_DoWork(sender, e);
					return;
				}
				MessageBox.Show(e1.ToString());
			}
		}

		private void initMenues_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!initSuccessful)
			{
				MessageBox.Show("Could not connect to Trac Server, aborting..");
				Close();
				return;
			}
			BindListToMenu("Version", versions, versionToolStripMenuItem);
			BindListToMenu("Milestone", milestones, milestonesToolStripMenuItem);
			BindListToMenu("Component", components_list, componentToolStripMenuItem);
			BindListToMenu("Priority", priorities, priorityToolStripMenuItem);
			BindListToMenu("Severity", severities, severityToolStripMenuItem);
			menu.Enabled = true;
			HideProgressBar();
		}

		private static void BindListToMenu(string title, string[] items, ToolStripMenuItem toolStripMenuRoot)
		{
			foreach (string itemName in items)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(itemName);
				toolStripMenuRoot.DropDownItems.Add(item);
				item.Click += delegate
				{
					foreach (ToolStripMenuItem menuItem in toolStripMenuRoot.DropDownItems)
					{
						menuItem.Checked = false;
					}
					item.Checked = true;
					toolStripMenuRoot.Text = string.Format("{0}: {1}", title, item.Text);
				};
			}
			if (toolStripMenuRoot.DropDownItems.Count > 0)
			{
				ToolStripMenuItem item = (ToolStripMenuItem) toolStripMenuRoot.DropDownItems[0];
				item.Checked = true;
				toolStripMenuRoot.Text = string.Format("{0}: {1}", title, item.Text);
			}
		}

		private void HideProgressBar()
		{
			pleaseWait.Visible = false;
			progressBar.Visible = false;
			progressTimer.Enabled = false;
		}

		private void ShowProgressBar()
		{
			pleaseWait.Visible = true;
			progressBar.Visible = true;
			progressTimer.Enabled = true;
		}

		private void MainView_Load(object sender, EventArgs e)
		{
			Text = Text + " @ " + Settings.Default.ServerAddress
			                              	.Replace("xmlrpc","")
			                              	.Replace("login/xmlrpc","");
			initMenues.RunWorkerAsync();
		}

		private void progressTimer_Tick(object sender, EventArgs e)
		{
			if (progressBar.Maximum <= progressBar.Value)
				progressBar.Value = progressBar.Minimum;
			progressBar.PerformStep();
		}

		private ITracXmlRpc BuildTracService()
		{
			ITracXmlRpc trac = XmlRpcProxyGen.Create<ITracXmlRpc>();
		    trac.KeepAlive = false;
			trac.Url = Settings.Default.ServerAddress;
			trac.Credentials = CredentialCache.DefaultNetworkCredentials;
			return trac;
		}


		private void Mandatory_TextChanged(object sender, EventArgs e)
		{
			submitToolStripMenuItem.Enabled = summary.Text != "" &&
			                                  (expected.Text != "" || actual.Text != "" || repreduce.Text != "");
		}

		private void TakeSnapshot_Click(object sender, EventArgs e)
		{
			using (TakeScreenShot snap = new TakeScreenShot())
			{
				Visible = false;
				snap.ShowDialog(this);
				Visible = true;
				BringToFront();
				if (snap.BackgroundImage != null)
				{
					snapShotImg.Image = snap.BackgroundImage;
				}
			}
			drawing.Enabled = true;
			drawing.FlatStyle = FlatStyle.Standard;
			snapShotImg.Cursor = DefaultCursor;
			snapShotComment.Enabled = true;
		}

		private void clearImg_Click(object sender, EventArgs e)
		{
			drawing.Enabled = false;
			snapShotComment.Text = "";
			snapShotComment.Enabled = false;
			snapShotImg.Image = null;
			snapShotImg.Cursor = DefaultCursor;
			shouldDraw = false;
		}

		private void drawing_Click(object sender, EventArgs e)
		{
			if (!shouldDraw)
			{
				shouldDraw = true;
				drawing.FlatStyle = FlatStyle.Flat;
				snapShotImg.Cursor = Cursors.Cross;
			}
			else
			{
				shouldDraw = false;
				drawing.FlatStyle = FlatStyle.Standard;
				snapShotImg.Cursor = DefaultCursor;
			}
		}

		private void snapShotImg_MouseDown(object sender, MouseEventArgs e)
		{
			if (!shouldDraw)
				return;
			currentlyDrawing = true;
			temporaryOutlineStart = snapShotImg.PointToClient(MousePosition);
			using (Graphics graphics = snapShotImg.CreateGraphics())
			{
				Rectangle rect = new Rectangle(temporaryOutlineStart, new Size(1, 1));
				graphics.DrawRectangle(Pens.Red, rect);
				graphics.Dispose();
			}
		}

		private void snapShotImg_MouseMove(object sender, MouseEventArgs e)
		{
			if (!currentlyDrawing)
				return;
			snapShotImg.Invalidate();
			using (Graphics graphics = snapShotImg.CreateGraphics())
			{
				Point current = snapShotImg.PointToClient(MousePosition);
				int height = current.Y - temporaryOutlineStart.Y;
				int width = current.X - temporaryOutlineStart.X;
				int xPos = temporaryOutlineStart.X;
				int yPos = temporaryOutlineStart.Y;
				if (height < 0)
				{
					height = Math.Abs(height);
					yPos = yPos - height;
				}
				if (width < 0)
				{
					width = Math.Abs(width);
					xPos = xPos - width;
				}
				temporaryOutline = new Rectangle(xPos, yPos, width, height);
				graphics.DrawRectangle(Pens.Red, temporaryOutline.Value);
				graphics.Dispose();
			}
		}

		private void snapShotImg_MouseUp(object sender, MouseEventArgs e)
		{
			currentlyDrawing = false;
			if (temporaryOutline.HasValue == false)
				return;
			using (Graphics g = Graphics.FromImage(snapShotImg.Image))
			{
				g.DrawRectangle(Pens.Red, temporaryOutline.Value);
			}
			snapShotImg.Image = snapShotImg.Image;
			temporaryOutline = null;
		}

		private void snapShotImg_Paint(object sender, PaintEventArgs e)
		{
			if (temporaryOutline.HasValue)
			{
				e.Graphics.DrawRectangle(Pens.Red, temporaryOutline.Value);
			}
		}

		private void submitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			submitToolStripMenuItem.Enabled = false; //protect from double click
			ShowProgressBar();
			TicketAttributes attributes = new TicketAttributes();
			attributes.cc = "";
			attributes.comment = "";
			attributes.keywords = "";
			attributes.component = GetSelected(componentToolStripMenuItem);
			attributes.milestone = GetSelected(milestonesToolStripMenuItem);
			attributes.severity = GetSelected(severityToolStripMenuItem);
			attributes.version = GetSelected(versionToolStripMenuItem);
			attributes.priority = GetSelected(priorityToolStripMenuItem);
			ITracXmlRpc trac = BuildTracService();
			string description =
				string.Format(@"
== Expected: ==

{0}

== Actual: ==

{1}

== Steps to repreduce: ==

{2}
",
				              expected.Text,
				              actual.Text,
				              repreduce.Text);
			int id = trac.TicketCreate(summary.Text, description, attributes);
			SendSnapShot(trac, id);
			HideProgressBar();
			using (CreatedTicket ct = new CreatedTicket(id, summary.Text))
				ct.ShowDialog(this);
			summary.Text = "";
			expected.Text = "";
			actual.Text = "";
			repreduce.Text = "";
			clearImg_Click(sender, e);
		}

		private void SendSnapShot(ITracXmlRpc trac, int id)
		{
			if (snapShotImg.Image == null)
				return;
			using (MemoryStream ms = new MemoryStream())
			{
				snapShotImg.Image.Save(ms, ImageFormat.Png);
				string fileName = "snapshot-" + id + ".png";
				trac.PutAttachment(id, fileName, snapShotComment.Text, ms.ToArray(), false);
			}
		}

		private string GetSelected(ToolStripMenuItem menuRoot)
		{
			foreach (ToolStripMenuItem item in menuRoot.DropDownItems)
			{
				if (item.Checked)
					return item.Text;
			}
			return "";
		}
	}
}