using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Generics;

namespace NHibernate.Generics.Tests
{
	public class Blog
	{
		PostsSet posts;
        EntitySet<User> _users;
        EntityDictionary<string, string> _attributes;

		int blog_id;

		public virtual int BlogID
		{
			get { return blog_id; }
			set { blog_id = value; }
		}
		string blog_name;

        public virtual string BlogName
		{
			get { return blog_name; }
			set { blog_name = value; }
		}

        public virtual IDictionary<string, string> Attributes
        {
            get { return _attributes; }
        }

		public virtual ICollection<Post> Posts
		{
			get { return posts; }
		}

        public virtual ICollection<User> Users
        {
            get { return _users; }
        }

		public Blog()
		{
            _attributes = new EntityDictionary<string, string>();
			posts = new PostsSet(
				delegate(Post p) { p.Blog = this; },
				delegate(Post p) { p.Blog = null; }
				);
            _users = new EntitySet<User>(
                delegate(User u) { u.Blogs.Add(this);},
                delegate(User u) { u.Blogs.Add(this);});
		}

		public Blog(string name) : this()
		{
			this.blog_name = name;
		}
	}
}
