namespace TracUI
{
	partial class TakeScreenShot
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// TakeScreenShot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.ControlBox = false;
			this.Name = "TakeScreenShot";
			this.Opacity = 0.35;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Double Click To Take Screen Shot";
			this.TopMost = true;
			this.SizeChanged += new System.EventHandler(this.TakeScreenShot_SizeChanged);
			this.DoubleClick += new System.EventHandler(this.TakeScreenShot_DoubleClick);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TakeScreenShot_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TakeScreenShot_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TakeScreenShot_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion

	}
}

