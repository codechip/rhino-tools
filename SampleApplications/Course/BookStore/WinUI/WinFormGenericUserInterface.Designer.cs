namespace BookStore.WinUI
{
    partial class WinFormGenericUserInterface
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
            this.theAmazingLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // theAmazingLayout
            // 
            this.theAmazingLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theAmazingLayout.Location = new System.Drawing.Point(0, 0);
            this.theAmazingLayout.Name = "theAmazingLayout";
            this.theAmazingLayout.Size = new System.Drawing.Size(292, 273);
            this.theAmazingLayout.TabIndex = 0;
            // 
            // WinFormGenericUserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.theAmazingLayout);
            this.Name = "WinFormGenericUserInterface";
            this.Text = "WinFormGenericUserInterface";
            this.Load += new System.EventHandler(this.WinFormGenericUserInterface_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel theAmazingLayout;

    }
}