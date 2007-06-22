using System;
using System.Drawing;
using Iesi.Collections.Generic;

namespace MyBlog
{
	public class User
	{
		private int id;
		private byte[] password;
		private string email;
		private string username;
		private DateTime createdAt;
		private string bio;

		private ISet<Blog> blogs = new HashedSet<Blog>();
		private ISet<Post> posts = new HashedSet<Post>();

		public virtual ISet<Blog> Blogs
		{
			get { return blogs; }
			set { blogs = value; }
		}

		public virtual ISet<Post> Posts
		{
			get { return posts; }
			set { posts = value; }
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual byte[] Password
		{
			get { return password; }
			set { password = value; }
		}

		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

		public virtual string Username
		{
			get { return username; }
			set { username = value; }
		}

		public virtual DateTime CreatedAt
		{
			get { return createdAt; }
			set { createdAt = value; }
		}

		public virtual string Bio
		{
			get { return bio; }
			set { bio = value; }
		}
	}
}