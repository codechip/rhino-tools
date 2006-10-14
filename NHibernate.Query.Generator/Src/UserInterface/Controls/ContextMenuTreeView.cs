using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.Controls
{
	/// <summary>
	/// Summary description for ContextMenuTreeView.
	/// </summary>
	public class ContextMenuTreeView : TreeView
	{
		private const UInt32 WM_CONTEXTMENU = 0x007B;
		Point contextMenuLocation;

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			contextMenuLocation = new Point(e.Node.Bounds.Left,e.Node.Bounds.Bottom+3);
		}

		protected override void WndProc(ref Message m)
		{
			if(m.Msg==WM_CONTEXTMENU)
			{
				if(ContextMenu!=null &&
					(m.LParam.ToInt32()==-1) && //invoked from keyboard
					SelectedNode!=null)
				{
					ContextMenu.Show(this,contextMenuLocation);
					return;
				}
			}
			base.WndProc(ref m);
		}
	}
}
