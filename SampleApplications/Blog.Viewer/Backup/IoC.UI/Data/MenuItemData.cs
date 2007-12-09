namespace IoC.UI.Data
{
	using System;
	using System.Windows.Forms;

	public class MenuItemData
	{
		private string text;
		private string command;
		private string parent;
		private Keys? shortcutKey;
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public string Command
		{
			get { return command; }
			set
			{
				if(string.IsNullOrEmpty(name))
					name = value;
				command = value;
			}
		}

		public string Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public Keys? ShortcutKey
		{
			get { return shortcutKey; }
		}

		public string Shortcut
		{
			get { return shortcutKey.ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					shortcutKey = null;
					return;
				}
				shortcutKey = (Keys)Enum.Parse(typeof(Keys), value);
			}
		}
	}
}
