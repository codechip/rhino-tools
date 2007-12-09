using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.Windows.Controls;
using Binsor.Presentation.Framework.Extensions;
using Binsor.Presentation.Framework.Data;
using System.Threading;

namespace Binsor.Presentation.Framework.Tests
{
    [TestFixture(ApartmentState = ApartmentState.STA)]
    public class MenuExtensionsFixture
    {
        [Test]
        public void WillAddItemToRootMenu()
        {
            Menu menu = new Menu { Name = "MainMenu" };
            menu.AddMenuItems(new MenuItemData { Parent = "MainMenu" });

            Assert.AreEqual(1, menu.Items.Count);
        }

        [Test]
        public void WillAddItemToNestedMenu()
        {
            Menu menu = new Menu { Name = "MainMenu" };
            MenuItem child = new MenuItem { Name = "Child" };
            menu.Items.Add(child);
            menu.AddMenuItems(new MenuItemData { Parent = "Child" });

            Assert.AreEqual(1, child.Items.Count);
        }

        [Test]
        public void WillAddItemToNestedMenu_TwoLevelsDeep()
        {
            Menu menu = new Menu { Name = "MainMenu" };
            MenuItem child = new MenuItem { Name = "Child" };
            MenuItem grandchild = new MenuItem { Name = "Grandchild" };
            menu.Items.Add(child);
            child.Items.Add(grandchild);
            menu.AddMenuItems(new MenuItemData { Parent = "Grandchild" });

            Assert.AreEqual(1, grandchild.Items.Count);
        }

        [Test]
        public void IfParentDoesNotExist_WillIgnoreAddition()
        {
            Menu menu = new Menu { Name = "MainMenu" };
            MenuItem child = new MenuItem { Name = "Child" };
            menu.Items.Add(child);
            menu.AddMenuItems(new MenuItemData { Parent = "Grandchild" });

            Assert.AreEqual(1, menu.Items.Count);
            Assert.AreEqual(0, child.Items.Count);
        }

    }
}
