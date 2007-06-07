using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BookStore.UI;

namespace BookStore.WinUI
{
    public partial class WinFormGenericUserInterface : Form, ISelectOptionsView
    {
        static bool isFirstTime = true;

        public WinFormGenericUserInterface()
        {
            InitializeComponent();
        }

        public void AddCommand(string commandName, Command cmd)
        {
            Button item = new Button();
            item.Text = commandName;
            item.Click += delegate {  cmd(); };
            theAmazingLayout.Controls.Add(item);
        }

        public void Display()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                Application.Run(this);
            }
            else
            {
                this.Show();
            }
        }

        private void WinFormGenericUserInterface_Load(object sender, EventArgs e)
        {
            Activate();
        }

        #region IView Members

        public void ShowError(string message, Exception e)
        {
            MessageBox.Show(message);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        #endregion
    }
}