using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Chapter5.Poster
{
    using System.IO;
    using System.Net;
    using Properties;

    public partial class PostUrlData : Form
    {
        public PostUrlData()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PostURL.Text = Settings.Default.URL;
            PostData.Text = Settings.Default.Data;
            PostData.LostFocus += delegate
            {
                Settings.Default.Data = PostData.Text;
                Settings.Default.Save();
            };
            PostURL.LostFocus += delegate
            {
                Settings.Default.URL = PostURL.Text;
                Settings.Default.Save();
            };
        }

        private void Post_Click(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create(PostURL.Text);
            request.Method = "POST";
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(PostData.Text);
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                using(StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    MessageBox.Show(sr.ReadToEnd());
                }
            }
            catch (WebException we)
            {
                using(StreamReader sr = new StreamReader(we.Response.GetResponseStream()))
                {
                    MessageBox.Show(sr.ReadToEnd());
                }
            }
        }
    }
}