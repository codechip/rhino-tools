using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Iesi.Collections.Generic;
using Model;

namespace Browser
{
	/// <summary>
	/// Interaction logic for CategoryViewer.xaml
	/// </summary>

	public partial class CategoryViewer : Page
	{
		private readonly Category category;
		protected List<Post> posts;

		public CategoryViewer(Category category)
		{
			Title = "Category: " + category.Name;
			this.category = category;
			InitializeComponent();
			Header.DataContext = category;
			posts = new List<Post>(category.Posts);
			posts.Sort(delegate(Post x, Post y) { return x.Modified.CompareTo(y.Modified); });
			PostsView.ItemsSource = posts;
		}

		public void ViewClicked(object sender, EventArgs e)
		{
			ICollectionView collectionView = CollectionViewSource.GetDefaultView(posts);
			Post post = (Post) collectionView.CurrentItem;
			PostViewer postViewer = new PostViewer(posts, collectionView.CurrentPosition, post);
			NavigationService.Navigate(postViewer);

		}
	}
}