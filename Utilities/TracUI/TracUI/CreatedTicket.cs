using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TracUI.Properties;

namespace TracUI
{
	public partial class CreatedTicket : Form
	{
		private readonly string title;
		private int ticketId;

		public CreatedTicket(int ticketId, string title)
		{
			InitializeComponent();
			this.ticketId = ticketId;
			this.title = title;
			lblMsg.Text = string.Format(lblMsg.Text, ticketId, title);
		}

		private void close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string url = string.Format(Settings.Default.TicketUrl, ticketId);
			Process.Start(url);
		}
	}
}