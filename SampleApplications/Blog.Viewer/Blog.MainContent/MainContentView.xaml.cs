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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Binsor.Presentation.Framework.Interfaces;

namespace Blog.MainContent
{
	/// <summary>
	/// Interaction logic for MainContentView.xaml
	/// </summary>
	public partial class MainContentView : UserControl, IViewMarker<MainContentView>
	{
		public MainContentView()
		{
			InitializeComponent();
		}
	}
}
