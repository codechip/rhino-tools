namespace TracUI
{
	partial class ScreenShot
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenShot));
			this.snapShotImg = new System.Windows.Forms.PictureBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.snapShotComment = new System.Windows.Forms.TextBox();
			this.lblComment = new System.Windows.Forms.Label();
			this.drawing = new System.Windows.Forms.Button();
			this.clearImg = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// snapShotImg
			// 
			this.snapShotImg.BackColor = System.Drawing.Color.Transparent;
			this.snapShotImg.Location = new System.Drawing.Point(0, 0);
			this.snapShotImg.Name = "snapShotImg";
			this.snapShotImg.Size = new System.Drawing.Size(618, 323);
			this.snapShotImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.snapShotImg.TabIndex = 3;
			this.snapShotImg.TabStop = false;
			this.snapShotImg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseDown);
			this.snapShotImg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseMove);
			this.snapShotImg.Paint += new System.Windows.Forms.PaintEventHandler(this.snapShotImg_Paint);
			this.snapShotImg.MouseUp += new System.Windows.Forms.MouseEventHandler(this.snapShotImg_MouseUp);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.snapShotComment);
			this.panel2.Controls.Add(this.lblComment);
			this.panel2.Controls.Add(this.drawing);
			this.panel2.Controls.Add(this.clearImg);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 334);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(621, 36);
			this.panel2.TabIndex = 2;
			// 
			// button2
			// 
			this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
			this.button2.Location = new System.Drawing.Point(6, 7);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(25, 25);
			this.button2.TabIndex = 4;
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.TakeSnapshot_Click);
			// 
			// snapShotComment
			// 
			this.snapShotComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.snapShotComment.Enabled = false;
			this.snapShotComment.Location = new System.Drawing.Point(125, 7);
			this.snapShotComment.Name = "snapShotComment";
			this.snapShotComment.Size = new System.Drawing.Size(410, 20);
			this.snapShotComment.TabIndex = 3;
			// 
			// lblComment
			// 
			this.lblComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblComment.AutoSize = true;
			this.lblComment.Location = new System.Drawing.Point(65, 10);
			this.lblComment.Name = "lblComment";
			this.lblComment.Size = new System.Drawing.Size(54, 13);
			this.lblComment.TabIndex = 2;
			this.lblComment.Text = "Comment:";
			// 
			// drawing
			// 
			this.drawing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.drawing.Enabled = false;
			this.drawing.Image = ((System.Drawing.Image)(resources.GetObject("drawing.Image")));
			this.drawing.Location = new System.Drawing.Point(37, 8);
			this.drawing.Name = "drawing";
			this.drawing.Size = new System.Drawing.Size(25, 25);
			this.drawing.TabIndex = 3;
			this.drawing.UseVisualStyleBackColor = true;
			this.drawing.Click += new System.EventHandler(this.drawing_Click);
			// 
			// clearImg
			// 
			this.clearImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clearImg.Location = new System.Drawing.Point(541, 8);
			this.clearImg.Name = "clearImg";
			this.clearImg.Size = new System.Drawing.Size(75, 23);
			this.clearImg.TabIndex = 0;
			this.clearImg.Text = "Clear";
			this.clearImg.UseVisualStyleBackColor = true;
			this.clearImg.Click += new System.EventHandler(this.clearImg_Click);
			// 
			// ScreenShot
			// 
			this.Controls.Add(this.snapShotImg);
			this.Controls.Add(this.panel2);
			this.Name = "ScreenShot";
			this.Size = new System.Drawing.Size(621, 370);
			((System.ComponentModel.ISupportInitialize)(this.snapShotImg)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox snapShotImg;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox snapShotComment;
		private System.Windows.Forms.Label lblComment;
		private System.Windows.Forms.Button drawing;
		private System.Windows.Forms.Button clearImg;

	}
}
