using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Model;
using Query;

namespace Browser
{
	/// <summary>
	/// Interaction logic for Browser.xaml
	/// </summary>

	public partial class BlogViewer : Page
	{
		private Category[] categories;

		public BlogViewer()
		{
			InitializeComponent();
			categories = Category.FindAll(OrderBy.Category.Name);
			DataContext = this.categories;
		}

		public void ViewClicked(object sender, EventArgs e)
		{
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(categories);
			Category category = (Category) collectionView.CurrentItem;
			CategoryViewer categoryViewer = new CategoryViewer(category);
			NavigationService.Navigate(categoryViewer);
		}
	}
}