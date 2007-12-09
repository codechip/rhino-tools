namespace IoC.UI.Util
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Windows.Forms;
	using Data;
	using Exceptions;

	public static class MenuExtensions
	{
		private static readonly OrderedDictionary finders = CreateFinders();
		private static readonly OrderedDictionary addToolStripItems = CreateAddToolStripItems();

		private static OrderedDictionary CreateAddToolStripItems()
		{
			OrderedDictionary dic = new OrderedDictionary();
			dic[typeof(ToolStrip)] = (AddMenuItemAction<ToolStrip>)delegate(ToolStrip parent, ToolStripItem child)
			{
				parent.Items.Add(child);
			};
			dic[typeof(ToolStripMenuItem)] = (AddMenuItemAction<ToolStripMenuItem>)delegate(ToolStripMenuItem parent, ToolStripItem child)
			{
				parent.DropDownItems.Add(child);
			};
			return dic;
		}

		delegate void AddMenuItemAction<TParent>(TParent parent, ToolStripItem child);
		delegate object Find<T>(T parent, string name);

		private static OrderedDictionary CreateFinders()
		{
			OrderedDictionary createFinders = new OrderedDictionary();
			createFinders[typeof(MenuStrip)] = (Find<MenuStrip>)delegate(MenuStrip menu, string name)
			{
				foreach (ToolStripItem item in menu.Items)
				{
					if (item.Name == name)
						return item;
					object recursed = FindFirst(item, name);
					if (recursed != null)
						return recursed;
				}
				return null;
			};
			createFinders[typeof(ToolStripItem)] = (Find<ToolStripMenuItem>)delegate(ToolStripMenuItem rootItem, string name)
			{
				foreach (ToolStripItem item in rootItem.DropDownItems)
				{
					if (item.Name == name)
						return item;
					object recursed = FindFirst(item, name);
					if (recursed != null)
						return recursed;
				}
				return null;
			};
			return createFinders;
		}

		private static object FindControl(Control ctrl, string name)
		{
			foreach (Control item in ctrl.Controls)
			{
				if (item.Name == name)
					return item;
				object recursed = FindFirst(item, name);
				if (recursed != null)
					return recursed;
			}
			return null;
		}

		private static object FindFirst(object obj, string name)
		{
			Delegate finder = GetFinderFor(obj.GetType());
			return finder.DynamicInvoke(obj, name);
		}

		private static Delegate GetFinderFor(Type c)
		{
			foreach (DictionaryEntry entry in finders)
			{
				Type type = (Type)entry.Key;
				if (type.IsAssignableFrom(c))
					return (Delegate)entry.Value;
			}
			return (Find<Control>) FindControl;
		}

		private static Delegate GetMenuAdderFor(Type c)
		{
			foreach (DictionaryEntry entry in addToolStripItems)
			{
				Type type = (Type)entry.Key;
				if (type.IsAssignableFrom(c))
					return (Delegate)entry.Value;
			}
			throw new UnknownControlTypeException("Could not find a menu adder for menu item of type: " + c + "");
		}

		public static void AddMenuItem(Control ctrl, MenuItemData data)
		{
			object parent = FindFirst(ctrl, data.Parent);
			AddMenuItemInternal(parent, data);
		}

		private static void AddMenuItemInternal(object parent, MenuItemData itemData)
		{
			if (parent == null)
				throw new ArgumentNullException("parent", "Could not find parent [" + itemData.Parent + "] for " + itemData.Command);

			ToolStripMenuItem item = new ToolStripMenuItem(itemData.Text);
			if (itemData.ShortcutKey != null)
				item.ShortcutKeys = itemData.ShortcutKey.Value;
			item.Name = itemData.Name;
			if (string.IsNullOrEmpty(itemData.Command) == false)
			{
				item.Click += delegate { Commands.Dispatch(itemData.Command); };
			}
			GetMenuAdderFor(parent.GetType()).DynamicInvoke(parent, item);
		}
	}
}