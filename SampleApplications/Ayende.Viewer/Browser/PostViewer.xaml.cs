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
			Title = "Post: " + post.Title;
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
			//TODO: Maybe able to do this with triggers?
			// yucky code, but I don't know how to get away with it
			if (position == 0)
			{
				Prev.Opacity = 0.25d;
			}
			if (position + 1 == posts.Count)
			{
				Next.Opacity = 0.25d;
			}
		}

		public void Prev_MouseDown(object sender, EventArgs e)
		{
			//TODO: couldn't find a way to disable MouseDown, so checking it here
			if (position == 0)
				return;
			NavigationService.Navigate(new PostViewer(posts, position - 1, posts[position - 1]));
		}
		public void Next_MouseDown(object sender, EventArgs e)
		{
			//TODO: couldn't find a way to disable MouseDown, so checking it here
			if (position + 1 == posts.Count)
				return;
			NavigationService.Navigate(new PostViewer(posts, position + 1, posts[position + 1]));
		}

		public void Category_Browse(object sender, EventArgs e)
		{
			TextBlock block = (TextBlock) sender;
			NavigationService.Navigate(new CategoryViewer(Category.Find(block.Tag)));
		}
	}
}