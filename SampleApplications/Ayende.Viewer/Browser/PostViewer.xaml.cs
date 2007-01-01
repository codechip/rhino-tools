using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Model;

namespace Browser
{
	/// <summary>
	/// Interaction logic for PostViewer.xaml
	/// </summary>
	public partial class PostViewer : Page
	{
		private readonly IList<Post> posts;
		private readonly int position;

		public PostViewer(IList<Post> posts, int position, Post post)
		{
			Title = WindowTitle = "Post: " + post.Title;
			this.posts = posts;
			this.position = position;
			InitializeComponent();
			DataContext = post;

			Categories.DataContext = post.Categories;

			//TODO: ?? apperantely there is no good way to load the data into a Frame except via Uri
			string template = @"
<html>
<body>
{0}
</body>
</html>
";
			File.WriteAllText(post.PostId + ".html", string.Format(template, post.Content));

			postFrame.Source = new Uri(string.Format("file:///{0}/{1}.html", Environment.CurrentDirectory, post.PostId));

			Categories.ItemsSource = post.Categories;
			if (position == 0)
			{
				Prev.Opacity = 0.25d;
				Prev.IsEnabled = false;
			}
			if (position + 1 == posts.Count)
			{
				Next.Opacity = 0.25d;
				Next .IsEnabled = false;
			}
		}

		public void Prev_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new PostViewer(posts, position - 1, posts[position - 1]));
		}
		public void Next_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new PostViewer(posts, position + 1, posts[position + 1]));
		}

		public void Category_Browse(object sender, EventArgs e)
		{
			TextBlock block = (TextBlock) sender;
			NavigationService.Navigate(new CategoryViewer(Category.Find(block.Tag)));
		}
	}
}