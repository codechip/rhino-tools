using System;
using Iesi.Collections.Generic;

namespace MyBlog
{
	public class Blog
	{
		private ISet<Post> posts = new HashedSet<Post>();
		private ISet<User> users = new HashedSet<User>();
		private ISet<Tag> tags = new HashedSet<Tag>();
		
		private int id;
		private string title;
		private string subtitle;
		private bool allowsComments;
		private DateTime createdAt;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ISet<Post> Posts
		{
			get { return posts; }
			set { posts = value; }
		}

		public virtual ISet<User> Users
		{
			get { return users; }
			set { users = value; }
		}

		public virtual ISet<Tag> Tags
		{
			get { return tags; }
			set { tags = value; }
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Subtitle
		{
			get { return subtitle; }
			set { subtitle = value; }
		}

		public virtual bool AllowsComments
		{
			get { return allowsComments; }
			set { allowsComments = value; }
		}

		public virtual DateTime CreatedAt
		{
			get { return createdAt; }
			set { createdAt = value; }
		}
	}
}