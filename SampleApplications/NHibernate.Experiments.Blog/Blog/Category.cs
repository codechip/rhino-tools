using Iesi.Collections.Generic;

namespace MyBlog
{
	public class Category
	{
		private int id;
		private ISet<Post> posts = new HashedSet<Post>();
		private string name;

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

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
	}
}