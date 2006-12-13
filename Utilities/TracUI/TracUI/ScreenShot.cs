using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TracUI
{
	public partial class ScreenShot : UserControl
	{
		private readonly Form form;
		private readonly TabPage tabPage;
		private bool shouldDraw;
		private Point temporaryOutlineStart;
		private bool currentlyDrawing;
		private Rectangle? temporaryOutline;
		
		public ScreenShot(Form form, TabPage tabPage)
		{
			this.form = form;
			this.tabPage = tabPage;
			InitializeComponent();
		}

		private void TakeSnapshot_Click(object sender, EventArgs e)
		{
			using (TakeScreenShot snap = new TakeScreenShot())
			{
				form.Visible = false;
				snap.ShowDialog(this);
				form.Visible = true;
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

		public void SendSnapShot(ITracXmlRpc trac, int id)
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

		public void Destroy()
		{
			clearImg_Click(this,EventArgs.Empty);
			TabControl control = (TabControl)tabPage.Parent;
			control.TabPages.Remove(tabPage);
			this.Dispose();
			tabPage.Dispose();
		}
	}
}
