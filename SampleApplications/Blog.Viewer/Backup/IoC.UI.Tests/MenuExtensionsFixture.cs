namespace IoC.UI.Tests
{
	using System.Windows.Forms;
	using Data;
	using Exceptions;
	using MbUnit.Framework;
	using Util;

	[TestFixture]
	public class MenuExtensionsFixture
	{
		[Test]
		public void WillAddMenuItemToMenuStrip()
		{
			Form f = new Form();
			
			MenuStrip ms = new MenuStrip();
			f.Controls.Add(ms);
			ms.Name = "MainMenu";
			MenuItemData data = new MenuItemData();
			data.Parent = "MainMenu";
			
			MenuExtensions.AddMenuItem(f, data);
			Assert.AreEqual(1, ms.Items.Count);
		}

		[Test]
		public void WillAddMenuItemToToolStripMenuItem()
		{
			Form f = new Form();

			MenuStrip ms = new MenuStrip();
			f.Controls.Add(ms);
			ms.Name = "MainMenu";
			ToolStripMenuItem item = new ToolStripMenuItem();
			item.Name = "Help";
			ms.Items.Add(item);
			MenuItemData data = new MenuItemData();
			data.Parent = "Help";

			MenuExtensions.AddMenuItem(f, data);
			Assert.AreEqual(1, item.DropDownItems.Count);
		}

		[Test]
		public void WillAddMenuItemToMenu_WhenParentIsNestedMenuItem()
		{
			Form f = new Form();

			MenuStrip ms = new MenuStrip();
			f.Controls.Add(ms);
			ms.Name = "MainMenu";
			ToolStripMenuItem item = new ToolStripMenuItem();
			item.Name = "Help";
			ms.Items.Add(item);
			ToolStripMenuItem community = new ToolStripMenuItem();
			community .Name = "Community";
			item.DropDownItems.Add(community);
			MenuItemData data = new MenuItemData();
			data.Parent = "Community";

			MenuExtensions.AddMenuItem(f, data);
			Assert.AreEqual(1, community.DropDownItems.Count);
		}

		[Test]
		[ExpectedException(typeof(UnknownControlTypeException), "Could not find a menu adder for menu item of type: System.Windows.Forms.TrackBar")]
		public void WillThrowIfFindUknownMenuType()
		{
			Form f = new Form();

			TrackBar value = new TrackBar();
			f.Controls.Add(value);
			Control community = new TrackBar();
			community.Name = "Community";
			value.Controls.Add(community);

			MenuItemData data = new MenuItemData();
			data.Parent = "Community";

			MenuExtensions.AddMenuItem(f, data);
			Assert.AreEqual(1, value.Controls.Count);
	
		}
	}
}