using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blog.Viewer.Shell
{
	using IoC.UI.Data;
	using IoC.UI.Interfaces;
	using IoC.UI.Util;

	public partial class ApplicationShell : Form, IShellView
	{
		public ApplicationShell()
		{
			InitializeComponent();
		}

		public void AddMenuItems(params MenuItemData[] items)
		{
			foreach (MenuItemData item in items)
			{
				MenuExtensions.AddMenuItem(this,item);
			}
		}
	}
}