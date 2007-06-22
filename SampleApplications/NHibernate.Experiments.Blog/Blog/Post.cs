using System;
using Iesi.Collections.Generic;

namespace MyBlog
{
	public class Post
	{
		private int id;
		private Blog blog;
		private User user;
		private string title;
		private string text;
		private DateTime postedAt;

		private ISet<Tag> tags = new HashedSet<Tag>();
		private ISet<Comment> comments = new HashedSet<Comment>();
		private ISet<Category> categories = new HashedSet<Category>();

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		public virtual User User
		{
			get { return user; }
			set { user = value; }
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}

		public virtual DateTime PostedAt
		{
			get { return postedAt; }
			set { postedAt = value; }
		}

		public virtual ISet<Comment> Comments
		{
			get { return comments; }
			set { comments = value; }
		}

		public virtual ISet<Category> Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public virtual ISet<Tag> Tags
		{
			get { return tags; }
			set { tags = value; }
		}


	}
}