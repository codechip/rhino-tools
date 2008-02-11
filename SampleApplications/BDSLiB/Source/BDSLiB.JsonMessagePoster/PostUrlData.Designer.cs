namespace Chapter5.Poster
{
    partial class PostUrlData
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
            this.Post = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PostURL = new System.Windows.Forms.TextBox();
            this.PostData = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Post
            // 
            this.Post.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Post.Location = new System.Drawing.Point(406, 366);
            this.Post.Name = "Post";
            this.Post.Size = new System.Drawing.Size(91, 24);
            this.Post.TabIndex = 0;
            this.Post.Text = "Post";
            this.Post.UseVisualStyleBackColor = true;
            this.Post.Click += new System.EventHandler(this.Post_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "URL:";
            // 
            // PostURL
            // 
            this.PostURL.Location = new System.Drawing.Point(62, 13);
            this.PostURL.Name = "PostURL";
            this.PostURL.Size = new System.Drawing.Size(435, 20);
            this.PostURL.TabIndex = 2;
            // 
            // PostData
            // 
            this.PostData.Location = new System.Drawing.Point(16, 63);
            this.PostData.Multiline = true;
            this.PostData.Name = "PostData";
            this.PostData.Size = new System.Drawing.Size(480, 288);
            this.PostData.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Message To Post:";
            // 
            // PostUrlData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 395);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PostData);
            this.Controls.Add(this.PostURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Post);
            this.Name = "PostUrlData";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Post;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PostURL;
        private System.Windows.Forms.TextBox PostData;
        private System.Windows.Forms.Label label2;
    }
}

