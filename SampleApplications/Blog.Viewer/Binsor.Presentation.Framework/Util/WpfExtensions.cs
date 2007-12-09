using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Binsor.Presentation.Framework.Data;

namespace Binsor.Presentation.Framework.Util
{
    public static class WpfExtensions
    {
        public static void AddMenuItems(this Menu menu, MenuItemData[] items)
        {
            foreach (var itemData in items)
            {
                var item = new MenuItem();
                item.Header = itemData.Header;
                item.Name = itemData.Name;
                item.Command = itemData.Command;
                if (menu.Name == itemData.Parent)
                    menu.Items.Add(item);
                else
                    AddToChildren(menu.Items, item, itemData.Parent);
            }
        }

        private static bool AddToChildren(ItemCollection itemCollection, MenuItem newItem, string parent)
        {
            foreach (MenuItem item in itemCollection)
            {
                if (item.Name == parent)
                {
                    item.Items.Add(newItem);
                    return true;
                }
                if (AddToChildren(item.Items, newItem, parent))
                    return true;
            }
            return false;
        }
    }
}
