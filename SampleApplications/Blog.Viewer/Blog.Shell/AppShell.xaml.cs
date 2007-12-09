using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Binsor.Presentation.Framework.Interfaces;
using Binsor.Presentation.Framework.Extensions;

namespace Blog.Shell
{
    using IoC = Rhino.Commons.IoC;
    using Binsor.Presentation.Framework.Data;
    using Rhino.Commons;

    /// <summary>
    /// Interaction logic for AppShell.xaml
    /// </summary>
    public partial class AppShell : Window, IApplicationShell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        public void AddMenuItems(params MenuItemData[] items)
        {
            MainMenu.AddMenuItems(items);
        }

        public void ApplicationShell_Initialized(object sender, EventArgs e)
        {
            IoC.Initialize(new RhinoContainer("assembly://Blog.Shell/Windsor.boo"));
            IoC.Container.Kernel.AddComponentInstance<IApplicationShell>(this);
        	IApplicationContext context = IoC.Resolve<IApplicationContext>();
        	context.Layouts.Register(Header);
			context.Layouts.Register(MainContent);
			context.Layouts.Register(Footer);
            context.Start();
        }

        private void Help_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Help? You make me laugh!" + Environment.NewLine +
                "You should debug this application with hex editor, blindfolded");
        }
    }
}
